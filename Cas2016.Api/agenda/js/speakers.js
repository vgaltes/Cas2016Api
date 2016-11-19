

Vue.component('speaker', {
    props: ['speakerData'],
    template: '<div class="row"> \
                <a :name=speakerData.id></a>\
            <div :id=speakerData.id class="col-md-3 limit"> \
                <img :src="speakerData.image"></img> \
            </div> \
            <div class="col-md-8" > \
                <p><h3>{{ speakerData.name }}</h3></p> \
                <p><span>\
                <a v-if="speakerData.website" :href="speakerData.website" target="_blank"><img src="http://cas2015.agile-spain.org/files/2015/09/icon-home-blue1.png" /> </a> \
                <a v-if="speakerData.twitterProfile" :href="speakerData.twitterProfile" target="_blank"><img src="http://cas2015.agile-spain.org/files/2015/09/icon-twitter-blue.png" /> </a> \
                <a v-if="speakerData.linkedinProfile" :href="speakerData.linkedinProfile" target="_blank"><img src="http://cas2015.agile-spain.org/files/2015/09/icon-linkedin-blue.png" /> </a> \
                </span></p>\
                <p>{{ speakerData.biography }} </p>\
                \
            </div> \
            </div> \
        </div>'
});

var speakers = new Vue({
    el: '#speakers',
    data: {
        speakers:[]
    },
    created: function () {
        var speakers = [];
        var app = this;
        $.getJSON("http://cas2016api.azurewebsites.net/speakers/",
            function (result) {
                $.each(result,
                    function(index, rawSpeaker) {
                        var speaker = {
                            id: rawSpeaker.id,
                            name: rawSpeaker.name,
                            image: rawSpeaker.image,
                            website: rawSpeaker.website,
                            twitterProfile: rawSpeaker.twitterProfile,
                            linkedinProfile: rawSpeaker.linkedinProfile,
                            biography: rawSpeaker.biography
                        };

                        speakers.push(speaker);
                    });
            });
        this.speakers = speakers;
    },
    updated: function() {
        this.scrollToHash(12);
    },
    methods: {
        scrollToHash: function () {
            var h = location.hash;
            location.hash = "initial";
            location.hash = h;
        }
    }
});

speakers.scrollTo(12);