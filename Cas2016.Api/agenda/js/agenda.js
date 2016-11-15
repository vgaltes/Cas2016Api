var toString = Object.prototype.toString;
var isFunction = function(o) { return toString.call(o) == "[object Function]"; };

function group(list, prop) {
    return list.reduce(function(grouped, item) {
        var key = isFunction(prop) ? prop.apply(this, [item]) : item[prop];
        grouped[key] = grouped[key] || [];
        grouped[key].push(item);
        return grouped;
    }, {});
}

// Returns a function which reverses the order of the
// arguments to the passed in function when invoked.
function flip(fn) {
    return function() {
        var args = [].slice.call(arguments);
        return fn.apply(this, args.reverse());
    };
}

// Returns a new function that curries the original
// function's arguments from right to left.
function rightCurry(fn, n) {
var arity = n || fn.length,
    fn = flip(fn);
    return function curried() {
        var args = [].slice.call(arguments),
            context = this;

        return args.length >= arity ?
            fn.apply(context, args.slice(0, arity)) :
            function () {
                var rest = [].slice.call(arguments);
                return curried.apply(context, args.concat(rest));
            };
    };
}

var groupBy = rightCurry(group); 

function getSpeakers(rawSpeakers){
    var speakers = [];
    $.each(rawSpeakers, function(i, rawSpeaker){
        var speaker = {
            name: rawSpeaker.name,
            id: rawSpeaker.id
        };
        speakers.push(speaker);
    });

    return speakers;
}

var eventHub = new Vue();

Vue.component('session',
{
    data: function() {
        return {
            activeColor: 'black'
        }
    },
    props: ['info'],
    template: '<div v-if="info" class="slot_container full" @click="openDetails(info)"> \
                        <p class="talk-title center" v-bind:style="{color: activeColor}">{{ info.title }}</p> \
                        <p class="talk-speakers center">{{ formatSpeakers(info.speakers) }}</p> \
                    </div>',
    methods: {
        openDetails: function(session) {
            eventHub.$emit('session-modal:open', session);
        },
        formatSpeakers: function (speakers) {
            if (!speakers) return "No Speaker";

            var speakersName = speakers.map(function (s) { return s.name });
            return speakersName.join();
        }
    },
    created: function () {
        if (!this.info) return;

        switch(this.info.room)
        {
            case 1:
                this.activeColor = "#D52244";
                break;
            case 2:
                this.activeColor = "#EB5C32";
                break;
            case 3:
                this.activeColor = "#00B7D4";
                break;
            case 4:
                this.activeColor = "#0A8CAA";
                break;
            case 5:
                this.activeColor = "#F59921";
                break;
        }
    }
});

Vue.component('agenda-day', {
    data: function(){
        return {
            slots: []
        }
    },
    props: ['url'],
    template: '<div> \
            <div class="row">\
                <div class="col-md-2"> </div> \
                <div class="col-md-2 center talk-title">Auditorio</div> \
                <div class="col-md-2 center talk-title">Gasteiz</div> \
                <div class="col-md-2 center talk-title">Avenida</div> \
                <div class="col-md-2 center talk-title">Micaela Portilla</div> \
                <div class="col-md-2 center talk-title">La Florida</div> \
            </div>\
            <div class="row is-flex" v-for="slot in slots"> \
                <div class="col-md-2"> \
                    {{ formatDate(slot.startTime) }} - {{ formatDate(slot.endTime) }} \
                </div> \
                <div class="col-md-10" v-if="slot.sessions[1][0] && slot.sessions[1][0].isPlenary === true"> \
                    <!-- <div class="talk-title center">{{ slot.sessions[1][0].title }} </div> \
                    <div class="talk-speakers center">{{ formatSpeakers(slot.sessions[1][0].speakers) }} </div> --> \
                    <session :info="slot.sessions[1][0]"> </session> \
                </div> \
                <div v-else class="col-md-2" v-for="(s, index) in slot.sessions" > \
                    <div v-if="s.length === 1"> \
                        <session :info="s[0]"></session> \
                    </div> \
                    <div v-else> \
                        <div class="slot_container half"> \
                            <session :info="s[0]"></session> \
                        </div> \
                        <div class="slot_container half"> \
                            <session :info="s[1]"></session> \
                        </div> \
                    </div> \
                </div> \
            </div> \
        </div>',
    created: function() {
        this.updateData();
    },
    methods: {
        openDetails: function(session) {
            eventHub.$emit('session-modal:open', session);
        },
        formatSpeakers: function (speakers) {
            if (!speakers) return "No Speaker";

            var speakersName = speakers.map(function (s) { return s.name });
            return speakersName.join();
        },
        updateData: function(){
            var apiSessions = [];
            var slots = [];
            $.getJSON(this.url, function(result){
                $.each(result, function(i, rawSession){
                    var session = {
                        id: rawSession.id,
                        title: rawSession.title,
                        speakers: getSpeakers(rawSession.speakers),
                        startTime: rawSession.startTime,
                        endTime: rawSession.endTime,
                        duration: rawSession.duration,
                        room: rawSession.room.id,
                        description: rawSession.description,
                        isPlenary: rawSession.isPlenary
                    };

                    apiSessions.push(session);
                });

                var normalSessions = apiSessions.filter(function(session){
                    return session.duration === 45 || session.isPlenary;
                });

                var sessionsGroupedByStartTimes = groupBy("startTime")(normalSessions);

                $.each(sessionsGroupedByStartTimes, function(i, ss){
                    slots.push({
                        startTime: ss[0].startTime,
                        endTime: ss[0].endTime,
                        sessions: []
                    });
                });

                var selectSlots = function(session){
                    return slots.filter(function(slot){
                        return session.startTime === slot.startTime || session.endTime === slot.endTime;
                    });
                }

                $.each(apiSessions, function(i, s){
                    var slots = selectSlots(s);
                    if (slots) {
                        $.each(slots,
                            function(i, slot) {
                                slot.sessions.push(s);
                            });
                    }
                });

                slots.sort(function(a, b){return new Date(a.startTime) - new Date(b.startTime);});

                var diff = function(a, b) {
                    return a.filter(function(i) {return b.indexOf(i) < 0;});
                };

                var insert = function (list, index, item) {
                    list[index] = item;
                };

                $.each(slots, function(i, slot){
                    slot.sessions.sort(function(a, b){return a.room - b.room});
                    slot.sessions = groupBy("room")(slot.sessions);
                    //rellenar huecos si no es penaria
                    var totalRooms = [1, 2, 3, 4, 5];
                    var rooms = [];
                    $.each(slot.sessions,
                        function(i, s) {
                            rooms.push(s[0].room);
                        });
                    var emptyRooms = diff(totalRooms, rooms);
                    if (emptyRooms < totalRooms.length - 1) {
                        $.each(emptyRooms,
                            function(i, r) {
                                insert(slot.sessions,
                                    r,
                                    {
                                        id: 1,
                                        title: 'test',
                                        speakers: [],
                                        startTime: '',
                                        endTime: '',
                                        duration: 45,
                                        room: r,
                                        description: 'test description',
                                        isPlenary: false
                                    });

                            });
                    }
                });
            });

            this.slots = slots;
        },
        formatDate: function(d){
            var date = new Date(d);
            return date.getHours() + ":" + (date.getMinutes() === 0 ? "00" : date.getMinutes());
        }
    }
});

var agenda = new Vue({
    el: '#agenda',
    data: {
        sessionDetails: null,
        speakers:[]
    },
    methods: {
        getSpeaker: function (speakerId) {
            var app = this;
            return $.getJSON("http://cas2016api.azurewebsites.net/speakers/" + speakerId, function (result) {
                var speaker = {
                    id: result.id,
                    name: result.name
                };

                app.speakers.push(speaker);
            });
        },
        showSessionDetails : function(session) {
            this.sessionDetails = session;
            this.speakers = [];
            var app = this;

            var requests = [];
            $.each(this.sessionDetails.speakers, function(i, speaker){
                requests.push(app.getSpeaker(speaker.id));
            });

            $.when.apply($, requests).done(function (schemas) {
                $("#session-modal").modal('toggle');
            });
        },
        getSpeakerLink: function(speakerId) {
            return "./speakers.html#" + speakerId;
        }
    },
    created: function () {
        eventHub.$on('session-modal:open', this.showSessionDetails);
    },

    beforeDestroy: function () {
        eventHub.$off('session-modal:open', this.showSessionDetails);
    }
});