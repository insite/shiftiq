// MathQuill
$(document).ready(function () {
    var MQ = MathQuill.getInterface(2);

    Sys.Application.add_load(init);

    init();

    function init() {
        $("span.math-eq").each(function (i, el) {
            MQ.StaticMath(el);
            $(el).removeClass("math-eq");
        });
    }
});

// Sticky Buttons Bottom Container
(function () {
    const containers = document.querySelectorAll('.sticky-buttons');
    if (containers.length == 0)
        return;

    const observer = new IntersectionObserver(
        observerCallback,
        {
            threshold: [1],
            rootMargin: "0% 100% 0% 100%"
        });

    for (let i = 0; i < containers.length; i++)
        observer.observe(containers[i]);

    $(function () {
        setTimeout(() => {
            for (let i = 0; i < containers.length; i++) {
                containers[i].classList.add('anim');
            }
        }, 50);
    });

    function observerCallback(entries) {
        for (let i = 0; i < entries.length; i++) {
            const entry = entries[i];
            const target = entry.target;

            target.classList.toggle('sticky', entry.intersectionRatio < 1);
        }
    }
})();