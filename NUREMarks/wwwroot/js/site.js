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

function SetRatingColor(Rating)
{
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

window.onload = function () {
    var rates = document.getElementsByClassName('rate');

    for (var i = 0; i < rates.length; i++) {
        rates[i].style.backgroundColor = SetRatingColor(rates[i].innerText.split(',')[0]);
    }
};


$('#search-field').keyup(function (e) {
    if (e.keyCode == 13) {
        showSearchResults();
    }
});

$('#search-button').click(function (e) {
    showSearchResults();
});


function showSearchResults() {
    window.location.href = '/Ratings/Search?name=' + $("#search-field").val();
}