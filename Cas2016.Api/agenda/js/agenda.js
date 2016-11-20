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
    template: '<div class="grid-item" @click="openDetails(info)"> \
                <p class="talk-room" v-bind:style="{color: activeColor}" > {{ info.room.name }} <p> \
                                <h4 class="talk-title" v-bind:style="{color: activeColor}">{{ info.title }}<img class="flag" :src="formatFlag()"></img></h4> \
                                <p class="talk-speakers">{{ formatSpeakers(info.speakers) }}</p> \
                            </div>',

    methods: {
        openDetails: function(session) {
            eventHub.$emit('session-modal:open', session);
        },
        formatSpeakers: function (speakers) {
            if (!speakers) return "No Speaker";

            var speakersName = speakers.map(function (s) { return s.name });
            return speakersName.join();
        },
        formatFlag: function() {
            return "../images/flag_" + this.info.language + ".jpg";
        }
    },
    created: function () {
        if (!this.info) return;

        switch(this.info.room.id)
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
    template: '<div class="agenda"> \
                    <div class="slot-container grid-col-small" v-for="slot in slots"> \
                        <h3 class="slot-time">{{ formatDate(slot.startTime) }} - {{ formatDate(slot.endTime) }} </h3> \
                            <div v-for="s in slot.sessions"> \
                                   <session :info="s"></session> \
                            </div> \
                    </div>    \
              <\div>'
    ,
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
                        room: { id: rawSession.room.id, name: rawSession.room.name },
                        description: rawSession.description,
                        isPlenary: rawSession.isPlenary,
                        language: rawSession.language,
                        extraInfo: rawSession.extraInfo
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

                slots.sort(function (a, b) { return new Date(a.startTime) - new Date(b.startTime); });

                $.each(slots,
                    function(i, slot) {
                        slot.sessions = slot.sessions.sort(function (a, b) { return a.room.id - b.room.id });
                    });
            });

            this.slots = slots;
        },
        formatDate: function (d) {
            var tz1 = moment(d).tz("Europe/Madrid");
            return tz1.format("HH:mm");
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
            return "./speakers.html?id=" + speakerId;
        }
    },
    created: function () {
        eventHub.$on('session-modal:open', this.showSessionDetails);
    },

    beforeDestroy: function () {
        eventHub.$off('session-modal:open', this.showSessionDetails);
    }
});