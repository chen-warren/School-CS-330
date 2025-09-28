/* Searching with Google API*/ 
function performSearch() {
    const query = $('#query').val();
    if (query) {
        const searchUrl = `https://www.googleapis.com/customsearch/v1?q=${query}&key=AIzaSyDRn2ABrcdRkr3e15t8NoAyNs_krJnb70w&cx=d2eb03f5769a64043`;

        $.get(searchUrl, function (data) {
            let resultsHtml = '';
            if (data.items) {
                data.items.forEach(item => {
                    resultsHtml += `
                        <div class="search-item">
                            <a href="${item.link}" target="_blank">
                                <div>
                                    <h3>${item.title}</h3>
                                    <p>${item.snippet}</p>
                                    <p>${formatUrl(item.link)}</p>
                                </div>
                            </a>
                        </div>
                    `;
                });
                $('#searchResults').html(resultsHtml).css("visibility", "visible");
            } else {
                $('#searchResults').html('<p>No results found</p>').css("visibility", "visible");
            }
        });
    }
}

/* Allow pressing 'Enter' to search */ 
document.getElementById('query').addEventListener("keypress", function(event) {
    if (event.key === "Enter") {
        performSearch();
    }
});

/* for I'm feeling lucky functionality */ 
function firstQuery() {
    const query = $('#query').val();
    if (query) {
        const searchUrl = `https://www.googleapis.com/customsearch/v1?q=${query}&key=AIzaSyDRn2ABrcdRkr3e15t8NoAyNs_krJnb70w&cx=d2eb03f5769a64043`;

        $.get(searchUrl, function (data) {
            if (data.items) {
                window.open(data.items[0].link, '_blank');
            } else {
                window.open('https://doodles.google/doodle/burning-man-festival/', '_blank');
            }
        });
    } else {
        window.open('https://doodles.google/doodle/burning-man-festival/', '_blank');
    }
}

/* background image switching */
let backgrounds = ['img2.jpg', 'img3.jpg', 'img4.jpg', 'img1.jpg'];
let attributes = 
    [
        `Photo by <a href="https://unsplash.com/@pawel_czerwinski?utm_content=creditCopyText&utm_medium=referral&utm_source=unsplash">Pawel Czerwinski</a> on <a href="https://unsplash.com/photos/background-pattern-FAlYVtV1kRg?utm_content=creditCopyText&utm_medium=referral&utm_source=unsplash">Unsplash</a>`, 
        `Photo by <a href="https://unsplash.com/@martz90?utm_content=creditCopyText&utm_medium=referral&utm_source=unsplash">Martin Martz</a> on <a href="https://unsplash.com/photos/a-dark-blue-and-orange-background-with-circles-OR8DEvVhym0?utm_content=creditCopyText&utm_medium=referral&utm_source=unsplash">Unsplash</a>`, 
        `Photo by <a href="https://unsplash.com/@martz90?utm_content=creditCopyText&utm_medium=referral&utm_source=unsplash">Martin Martz</a> on <a href="https://unsplash.com/photos/a-purple-and-orange-abstract-background-with-curves-My3Hal_y4FE?utm_content=creditCopyText&utm_medium=referral&utm_source=unsplash">Unsplash</a>`,
        `Photo by <a href="https://unsplash.com/@sebastiansvenson?utm_content=creditCopyText&utm_medium=referral&utm_source=unsplash">Sebastian Svenson</a> on <a href="https://unsplash.com/photos/brown-cardboard-box-with-yellow-light-d2w-_1LJioQ?utm_content=creditCopyText&utm_medium=referral&utm_source=unsplash">Unsplash</a>`,
    ]
let backgroundIndex = 0;
$('#background-change').click(function () {
    backgroundIndex = (backgroundIndex + 1) % backgrounds.length;
    $('body').css('background-image', `url(${backgrounds[backgroundIndex]})`);
    document.getElementById('attr').innerHTML = attributes[backgroundIndex];
});


/* showing current time */ 
$('#timeBtn').click(function () {
    const currentTime = new Date().toLocaleTimeString([], {hour: '2-digit', minute: '2-digit'});
    $('#current-time').text(currentTime);
    $('#time').dialog({
        modal: true, width: 250, height: 150
    }).css("visibility", "visible");
});


/* url formatter to display after performing search */ 
function formatUrl(url) {
    let formattedUrl = url;
    let urlParts = formattedUrl.split('/');
    let domain = urlParts[2];
    let path = urlParts.slice(3).join("→");
    if (path) {
        return `${formattedUrl.split('/')[0]}//${domain} → ${path}`;
    } else {
        return `${formattedUrl.split('/')[0]}//${domain}`;
    }
}





