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

Vue.component('agenda-day', {
    data: function(){
        return {
            slots: []
        }
    },
    props: ['url'],
    template: '<div> \
            <div class="row" v-for="slot in slots"> \
            <div class="col-xs-2 col-sm-2"> \
                {{ formatDate(slot.startTime) }} - {{ formatDate(slot.endTime) }} \
            </div> \
            <div class="panel panel-default col-xs-2 col-sm-2" v-for="session in slot.sessions" > \
                <div v-if="session.length === 1"> \
                    <div class="slot_container full"> \
                        <div class="panel-heading">{{ session[0].title }}</div> \
                        <div class="panel-body">{{ formatSpeakers(session[0].speakers) }}</div> \
                    </div> \
                </div> \
                <div v-else> \
                    <div class="slot_container half"> \
                        <div class="panel-heading">{{ session[0].title }}</div> \
                        <div class="panel-body">{{ formatSpeakers(session[0].speakers) }}</div> \
                    </div> \
                    <div class="slot_container half"> \
                        <div class="panel-heading">{{ session[1].title }}</div> \
                        <div class="panel-body">{{ formatSpeakers(session[1].speakers) }}</div> \
                    </div> \
                </div> \
            </div> \
            </div> \
        </div>',
    created: function(){
        this.updateData()
    },
    methods: {
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
                        room: rawSession.room.id
                    };

                    apiSessions.push(session);
                });

                var normalSessions = apiSessions.filter(function(session){
                    return session.duration === 45;
                });

                var sessionsGroupedByStartTimes = groupBy("startTime")(normalSessions);

                $.each(sessionsGroupedByStartTimes, function(i, ss){
                    slots.push({
                        startTime: ss[0].startTime,
                        endTime: ss[0].endTime,
                        sessions: []
                    });
                });

                var selectSlot = function(session){
                    return slots.filter(function(slot){
                        return session.startTime === slot.startTime || session.endTime === slot.endTime;
                    })[0];
                }

                $.each(apiSessions, function(i, s){
                    var slot = selectSlot(s);
                    if ( slot )  {
                        slot.sessions.push(s);
                    }
                });

                slots.sort(function(a, b){return new Date(a.startTime) - new Date(b.startTime);});

                $.each(slots, function(i, slot){
                    slot.sessions.sort(function(a, b){return a.room - b.room});
                    slot.sessions = groupBy("room")(slot.sessions);
                });
            });

            this.slots = slots;
        },
        formatDate: function(d){
            var date = new Date(d);
            return date.getHours() + ":" + ( date.getMinutes() === 0 ? "00" : date.getMinutes() )
        },
        formatSpeakers: function (speakers) {
            if (!speakers) return "No Speaker";

            var speakersName = speakers.map(function (s) { return s.name });
            return speakersName.join();
        }
    }
});

var agenda = new Vue({
    el: '#agenda'
});