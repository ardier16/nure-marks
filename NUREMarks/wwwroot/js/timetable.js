function setTable() {
    var date = new Date(2017, 1, 6);
    var options = {
      month: 'numeric',
      day: 'numeric',
      weekday: 'short',
    };

    var th = $("#timetable-th")[0];
    var trs = $(".timetable-tr");

    for (var i = 0; i < 150; i++) {
        th.innerHTML += '<th class="text-center">' + date.toLocaleString("ru", options) + '</th>';
        date.setDate(date.getDate() + 1);

            trs[0].innerHTML += '<td class="text-center">&nbsp;</td>';

    }
}

var isPairScroll = false;

function setTimeTable() {

    if ($("#htmlText")[0])
    {
        var date = new Date(2017, 1, 6);

        var obj = JSON.parse($("#htmlText")[0].value);

        var today = new Date();
        var dt = new Date(obj.events[0].start_time * 1000);
        dt.setHours(0);
        dt.setMinutes(0);
        dt.setSeconds(0);
        var d = ~~((today.getTime() - dt.getTime()) / 1000 / 3600 / 24);
        var scr = 141 * d;

        for (var i = 0; i < obj.events.length; i++) {
            var number = obj.events[i].number_pair;
            var day = new Date(obj.events[i].start_time * 1000);
            var delta = day.getTime() - date.getTime();
            var m = ~~(delta / 1000 / 3600 / 24);

            var type, typeClass, fullType, teacher, groups, name;

            switch (obj.events[i].type)
            {
                case 0:
                    type = "Лк";
                    fullType = "Лекцiя";
                    typeClass = "lecture";
                    break;
                case 10:
                    type = "Пз";
                    fullType = "Практичне заняття";
                    typeClass = "practice";
                    break;
                case 21:
                    type = "Лб";
                    fullType = "Лабораторна IОЦ";
                    typeClass = "laboratory";
                    break;
                case 53:
                    type = "IспКо";
                    fullType = "Iспит комбiнований";
                    typeClass = "exam";
                    break;
                case 30:
                    type = "Конс";
                    fullType = "Консультацiя";
                    typeClass = "consultation";
                    break;
            }

            var subject = findSubject(obj, obj.events[i].subject_id);
            var auditory = obj.events[i].auditory;
            name = subject.title;
            groups = findGroups(obj, obj.events[i].groups);
            teacher = findTeacher(obj, obj.events[i].teachers[0]);

            var tr = $(".timetable-tr")[number-1];
            var td = tr.children[m];

            td.className = "";

            var modal = getPairInfoDiv(fullType, name, auditory, teacher, groups);

            var pairInfo = '<div class="pair ' + typeClass + ' open_modal' +
                '"><input type="hidden" value="#modal' + i + '"><p>' + subject.brief + '</p><p>' + type + ' ' + auditory + '</p>' +
                '<div id="modal' + i +  '" class="modal_div ' + typeClass + '">' + modal +
                '</div></div>';

            if (td.firstChild.innerHTML !== "&nbsp;")
            {
                var div = td.firstChild;
                div.className = "multiple " + div.className;

                div.onmouseover = function(e) {
                    isPairScroll = true;
                }

                div.onmouseout = function(e) {
                    isPairScroll = false;
                }

                div.innerHTML +=  '<br />' + pairInfo;
            }
            else
            {
                td.firstChild.innerHTML = pairInfo;
            }


        }

        $("#timetable-block").scrollLeft(scr);

        var trs = $(".timetable-tr");

        for (var i = 0; i < trs.length; i++) {
            for (var j = 0; j < d; j++) {
                trs[i].children[j].className += " lastpair-td";
            }
        }
    }

    showPairInfo();

}

function getPairInfoDiv(type, name, aud, teacher, groups) {
    return '<span class="modal_close">X</span><p class="pair-title">' + name + '</p><hr><div class="pair-desc"><p>Тип</p><p>Аудитория</p>' +
                '<p>Преподаватель</p><p>Группы</p></div><div class="pair-info"><p>' + type + '</p><p>' + aud +
                '</p><p>' + teacher + '</p><p>' + groups + '</p></div>';
}

function showPairInfo() {
    var overlay = $('#overlay'); // пoдлoжкa, дoлжнa быть oднa нa стрaнице
    var open_modal = $('.open_modal'); // все ссылки, кoтoрые будут oткрывaть oкнa
    var close = $('.modal_close, #overlay'); // все, чтo зaкрывaет мoдaльнoе oкнo, т.е. крестик и oверлэй-пoдлoжкa
    var modal = $('.modal_div'); // все скрытые мoдaльные oкнa

     open_modal.click( function(event){ // лoвим клик пo ссылке с клaссoм open_modal
         event.preventDefault(); // вырубaем стaндaртнoе пoведение
         var div = $(this)[0].children[0].value; // вoзьмем стрoку с селектoрoм у кликнутoй ссылки
         overlay.fadeIn(400, //пoкaзывaем oверлэй
             function(){ // пoсле oкoнчaния пoкaзывaния oверлэя
                 $(div) // берем стрoку с селектoрoм и делaем из нее jquery oбъект
                     .css('display', 'block')
                     .animate({opacity: 1, top: '50%'}, 200); // плaвнo пoкaзывaем
         });
     });

     close.click( function(){ // лoвим клик пo крестику или oверлэю
            modal // все мoдaльные oкнa
             .animate({opacity: 0, top: '45%'}, 200, // плaвнo прячем
                 function(){ // пoсле этoгo
                     $(this).css('display', 'none');
                     overlay.fadeOut(400); // прячем пoдлoжку
                 }
             );
     });
}

function findSubject(json, id) {
    for (var i = 0; i < json.subjects.length; i++) {
        if (json.subjects[i].id === id)
            return json.subjects[i];
    }
}

function findTeacher(json, id) {
    for (var i = 0; i < json.teachers.length; i++) {
        if (json.teachers[i].id === id)
            return json.teachers[i].full_name;
    }

    return "-";
}

function findGroups(json, idxs) {
    var res = "";
    var c = 0;

    for (var j = 0; j < idxs.length; j++) {
        for (var i = 0; i < json.subjects.length; i++) {
            if (json.groups[i].id === idxs[j]) {
                res += json.groups[i].name + "; ";
                c++;
            }

            if (c == 3) {
                res += "\n";
                c = 0;
            }
        }
    }

    return res.substr(0, res.length-2);
}

document.ready = function () {
    //setTable();
    setTimeTable();
}

$("#timetable-block")[0].onwheel = function(e) {
    if (!isPairScroll) {
        var x = $("#timetable-block").scrollLeft();
        if (e.deltaY > 0)
            $("#timetable-block").animate({scrollLeft: x + 282}, 200);
        else
            $("#timetable-block").animate({scrollLeft: x - 282}, 200);
    }
}
