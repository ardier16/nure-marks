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
    var rates = $('.rate');
    var marks = $('.mark');

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
            $('html, body').animate({
                scrollTop: $("tr[data-id='" + id + "']").offset().top
            }, 200);
        }
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


function calculateRating() {
    var credits = $(".subject-input.credits");
    var marks = $(".subject-input.mark-value");
    var additional = +$(".subject-input.additional")[0].value;
    var rating = 0;
    var crSum = 0;

    for (var i = 0; i < credits.length; i++) {
        if (credits[i].parentElement.className.indexOf("last") == -1) {
            crSum += +credits[i].value;
            rating += credits[i].value * marks[i].value;
        }
    }

    rating = 0.9 * rating / crSum + additional;

    $(".calc-rating")[0].innerHTML = "Ваш рейтинг: <div class='rate subject-blocks'>" + Math.round(rating*100)/100 + "</div>";
    $('.rate')[0].style.backgroundColor = SetRatingColor(rating)
}

$(".calculate-form")[0].onsubmit = function() {
    calculateRating();
    return false;
}



$(".last")[0].children[3].onfocus = function() {
    addSubject();
    var subBlocks = $(".subject");
    setTimeout(function() { subBlocks[subBlocks.length - 2].children[3].focus() }, 30);
}

$(".subject-blocks")[0].onkeyup = function() {
    drawMarksValues();
}

$(".subject-blocks")[0].onclick = function() {
    drawMarksValues();
}

function drawMarksValues() {
    var marks = $(".subject-input.mark-value");

    for (var i = 0; i < marks.length - 1; i++) {
        if (marks[i].value != "") {
            marks[i].style.backgroundColor = SetMarkColor(marks[i].value);
            marks[i].style.color = "white";
        }
    }
}

function deleteSubject(index) {
    var subBlock = $(".subject-blocks")[0];
    subBlock.children[index-1].remove();

    for (var i = index - 1; i < subBlock.children.length; i++) {
        subBlock.children[i].children[0].innerHTML = i + 1;
    }
}

function addSubject() {
    var subBlock = $(".subject-blocks")[0];
    var count = subBlock.children.length;

    var div = document.createElement('div');
    div.className = "subject";
    div.innerHTML = '<div class="subject-number">' + count + '</div>' +
                     '<div class="subject-delete" onclick="deleteSubject(' + count + ')">x</div>' +
                     '<p class="subject-name">Название</p>' +
                     '<input type="text" class="subject-input name" maxlength="17" placeholder="Предмет">' +
                     '<p class="subject-label">Кредиты</p>' +
                     '<input type="number" class="subject-input credits" maxlength="0" min="0" max="6" step="0.5" placeholder="0-6" required checked>' +
                     '<p class="subject-label">Оценка</p>' +
                     '<input type="number" class="subject-input mark-value" maxlength="0" min="60" max="100" placeholder="60-100" required checked>';

    subBlock.insertBefore(div, subBlock.children[count - 1]);
    $(".subject-number")[count].innerHTML = count + 1;
}
