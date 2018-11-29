var timetable = {};
var isPairScroll = false;

window.onload = function () {
    setTimeTable();
}

function setTimeTable() {
    timetable = JSON.parse($("#htmlText")[0].value);
    generateTable(timetable.events[0].start_time, timetable.events[timetable.events.length - 1].start_time);
    setPairs();
    $("#timetable-block").scrollLeft(120 * getDaysDifference());
    hidePastPairs(getDaysDifference());
    showPairInfo();
}

function setPairs() {
    var date = new Date(timetable.events[0].start_time * 1000);
    date.setHours(0);
    date.setMinutes(0);
    date.setSeconds(0);

    for (var i = 0; i < timetable.events.length; i++) {
        var number = timetable.events[i].number_pair;
        var day = new Date(timetable.events[i].start_time * 1000);
        var delta = day.getTime() - date.getTime();
        var m = ~~(delta / 1000 / 3600 / 24);

        var pair = getPair(timetable.events[i]);

        var tr = $(".timetable-tr")[number - 1];
        var td = tr.children[m];

        td.className = "";
        var modal = getPairInfoDiv(pair);

        var pairInfo = '<div class="pair ' + pair.typeClass + ' open_modal' +
            '"><input type="hidden" value="#modal' + i + '"><p>' + pair.subject.brief + '</p><p>' + pair.type + ' ' + pair.auditory + '</p>' +
            '<div id="modal' + i + '" class="modal_div ' + pair.typeClass + '">' + modal +
            '</div></div>';

        if (td.firstChild.innerHTML !== "&nbsp;") {
            var div = td.firstChild;
            div.className = "multiple " + div.className;

            div.onmouseover = function (e) {
                isPairScroll = true;
            }

            div.onmouseout = function (e) {
                isPairScroll = false;
            }

            div.innerHTML += '<br />' + pairInfo;
        }
        else {
            td.firstChild.innerHTML = pairInfo;
        }
    }
}

function getPair(event) {
    var pair = {};
    getPairType(event.type, pair);
    pair.teacher = findTeacher(event.teachers[0]);
    pair.subject = findSubject(event.subject_id);
    pair.auditory = event.auditory;
    pair.name = pair.subject.title;
    pair.groups = findGroups(event.groups);

    return pair;
}

function getPairType(typeId, pair) {
    switch (typeId) {
        case 0:
            pair.type = "Лк";
            pair.fullType = "Лекцiя";
            pair.typeClass = "lecture";
            return;
        case 10:
            pair.type = "Пз";
            pair.fullType = "Практичне заняття";
            pair.typeClass = "practice";
            return;
        case 21:
        case 22:
            pair.type = "Лб";
            pair.fullType = "Лабораторна IОЦ";
            pair.typeClass = "laboratory";
            return;
        case 53:
            pair.type = "Iспит";
            pair.fullType = "Iспит комбiнований";
            pair.typeClass = "exam";
            return;
        case 30:
            pair.type = "Конс";
            pair.fullType = "Консультацiя";
            pair.typeClass = "consultation";
            return;
        case 40:
            pair.type = "Зал";
            pair.fullType = "Залік";
            pair.typeClass = "exam";
            return;
    }
}

function getDaysDifference() {
    var today = new Date();
    var dt = new Date(timetable.events[0].start_time * 1000);
    dt.setHours(0);
    dt.setMinutes(0);
    dt.setSeconds(0);

    return ~~((today.getTime() - dt.getTime()) / 1000 / 3600 / 24);
}

function hidePastPairs(days) {

    var trs = $(".timetable-tr");

    for (var i = 0; i < trs.length; i++) {
        for (var j = 0; j < trs[i].cells.length && j < days; j++) {
            trs[i].children[j].className += " lastpair-td";
        }
    }
}

function generateTable(startDate, endDate) {
    var th = '';
    var td = '';

    var date = new Date(startDate * 1000);

    date.setHours(7);
    date.setMinutes(0);
    date.setSeconds(0);

    startDate = date.getTime() / 1000;

    var current = new Date();

    if (current.getTime() > endDate * 1000) {
        endDate = (current.getTime() / 1000) + 6 * 24 * 3600;
    }

    while (startDate < endDate) {
        th += '<th class="text-center">' + new Date(startDate * 1000).toLocaleDateString() + '</th>';
        td += '<td class="text-center"><div class="pairs-block">&nbsp;</div></td>';
        startDate += 3600 * 24;
    }

    document.getElementById("timetable-th").innerHTML = th;
    var tds = document.getElementsByClassName("timetable-tr");

    for (let i = 0; i < tds.length; i++) {
        tds[i].innerHTML = td;
    }
}

function getPairInfoDiv(pair) {
    return '<span class="modal_close">X</span><p class="pair-title">' + pair.name + '</p><hr><div class="pair-desc"><p>Тип</p><p>Аудитория</p>' +
        '<p>Преподаватель</p><p>Группы</p></div><div class="pair-info"><p>' + pair.fullType + '</p><p>' + pair.auditory +
        '</p><p>' + pair.teacher + '</p><p>' + pair.groups + '</p></div>';
}

function showPairInfo() {
    var overlay = $('#overlay');
    var open_modal = $('.open_modal');
    var close = $('.modal_close, #overlay');
    var modal = $('.modal_div');

    open_modal.click(function (event) {
        event.preventDefault();
        var div = $(this)[0].children[0].value;
        overlay.fadeIn(400,
            function () {
                $(div)
                    .css('display', 'block')
                    .animate({ opacity: 1, top: '50%' }, 200);
            });
    });

    close.click(function () {
        modal
            .animate({ opacity: 0, top: '45%' }, 200,
            function () {
                $(this).css('display', 'none');
                overlay.fadeOut(400);
            }
            );
    });
}

function findSubject(id) {
    for (var i = 0; i < timetable.subjects.length; i++) {
        if (timetable.subjects[i].id === id) {
            return timetable.subjects[i];
        }
    }
}

function findTeacher(id) {
    for (var i = 0; i < timetable.teachers.length; i++) {
        if (timetable.teachers[i].id === id)
            return timetable.teachers[i].full_name;
    }

    return "-";
}

function findGroups(idxs) {
    var res = "";
    var c = 0;

    for (var j = 0; j < idxs.length; j++) {
        for (var i = 0; i < timetable.groups.length; i++) {
            if (timetable.groups[i].id === idxs[j]) {
                res += timetable.groups[i].name + "; ";
                c++;
            }

            if (c == 3) {
                res += "\n";
                c = 0;
            }
        }
    }

    return res.substr(0, res.length - 2);
}

$("#timetable-block")[0].onwheel = function (e) {
    if (!isPairScroll) {
        var x = $("#timetable-block").scrollLeft();
        if (e.deltaY > 0)
            $("#timetable-block").animate({ scrollLeft: x + 282 }, 50);
        else
            $("#timetable-block").animate({ scrollLeft: x - 282 }, 50);

        e.preventDefault();

    }
}