var selectedLi = document.getElementsByClassName('highlight')[0];
showDeps('first');

if ($('#course-list')) {
    $('#course-list').click(function (event) {
        var target = event.target;

        if (target.tagName !== "LI")
            return;

        highlight(target);
    });
}

function highlight(node) {
	if (selectedLi)
		selectedLi.classList.remove('highlight');

	selectedLi = node;
    selectedLi.classList.add('highlight');

    $(".panel-group").hide();
    $(".panel-group").fadeIn(500);

	selectedLi.onmouseover = null;

	var course = selectedLi.innerText[0];

	switch (course)
	{
		case '1':
			showDeps('first');
			return;
		case '2':
			showDeps('second');
			return;
		case '3':
			showDeps('third');
			return;						
		case '4':
			showDeps('fourth');
			return;
		case '5':
			showDeps('fifth');
			return;
		case '6':
			showDeps('sixth');
			return;
		default:
			return;															
	}
}

function showDeps(course)
{
    $(".dep").hide();
    $("." + course).css("display", "inline-block")
}

function SetRatingColor(Rating) {
    if (Rating >= 92)
        return "rgb(5, 111, 51)";
    if (Rating >= 85)
        return "rgb(23, 152, 46)";
    if (Rating >= 80)
        return "rgb(141, 193, 83)";
    if (Rating >= 75)
        return "rgb(246, 187, 67)";
    if (Rating >= 70)
        return "rgb(231, 126, 35)";
    if (Rating >= 60)
        return "rgb(233, 87, 62)";

    return "rgb(131, 131, 131)";
}

function SetMarkColor(Rating) {
    if (Rating >= 96)
        return "rgb(5, 111, 51)";
    if (Rating >= 90)
        return "rgb(23, 152, 46)";
    if (Rating >= 80)
        return "rgb(141, 193, 83)";
    if (Rating >= 75)
        return "rgb(246, 187, 67)";
    if (Rating >= 70)
        return "rgb(231, 126, 35)";
    if (Rating >= 60)
        return "rgb(233, 87, 62)";

    return "rgb(131, 131, 131)";
}

window.onload = function () {
    var rates = document.getElementsByClassName('rate');
    var marks = document.getElementsByClassName('mark');

    for (var i = 0; i < rates.length; i++) {
        rates[i].style.backgroundColor = SetRatingColor(rates[i].innerText.split(',')[0]);
    }

    for (i = 0; i < marks.length; i++) {
        marks[i].style.backgroundColor = SetMarkColor(marks[i].innerText);
    }

    if ($(".me")[0])
    {
        var id = $("tr.me")[0].dataset.id;

        if (id > 3)
        {
            id -= 3;
        }

        $('html, body').animate({
            scrollTop: $("tr[data-id='" + id + "']").offset().top
        }, 200);
    }
};


$('#search-field').keyup(function (e) {
    if (e.keyCode === 13) {
        showSearchResults();
    }
});

$('#search-button').click(function (e) {
    showSearchResults();
});


function showSearchResults() {
    window.location.href = '/Ratings/Search?name=' + $("#search-field").val();
}


function setTimeTable() {

    if ($("#htmlText")[0])
    {
        var obj = JSON.parse($("#htmlText")[0].value);

        for (var i = 0; i < obj.events.length; i++) {
            var number = obj.events[i].number_pair;
            var day = new Date(obj.events[i].start_time * 1000).getDay();
            var type, typeClass;

            switch (obj.events[i].type)
            {
                case 0:
                    type = "Лк";
                    typeClass = "lecture";
                    break;
                case 10:
                    type = "Пз";
                    typeClass = "practice";
                    break;
                case 21:
                    type = "Лб";
                    typeClass = "laboratory";
                    break;
            }

            var subject = findSubject(obj, obj.events[i].subject_id);
            var auditory = obj.events[i].auditory;

            var tr = $("tr")[number];
            var td = tr.children[day];

            td.className = "";

            var pairInfo = '<div class="pair ' + typeClass +
                '"><p>' + subject + '</p><p>' + type + ' ' + auditory + '</p></div>';

            if (td.innerHTML !== "&nbsp;")
            {
                td.className = "multiple";
                td.innerHTML += '<br />' + pairInfo;
            }
            else
            {
                td.innerHTML = pairInfo;
            }


            
        }
    }
    
}

function findSubject(json, id) {
    for (var i = 0; i < json.subjects.length; i++) {
        if (json.subjects[i].id === id)
            return json.subjects[i].brief;
    }
}

document.ready = function () {
    setTimeTable();
}