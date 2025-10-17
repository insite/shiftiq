import { useEffect, useRef } from "react";

const offsetFromTop = 450;

export default function ScrollTopButton() {
    const linkRef = useRef<HTMLAnchorElement>(null);
    const circleRef = useRef<SVGCircleElement>(null);

    useEffect(() => {
        if (!linkRef.current || !circleRef.current) {
            return;
        }

        const progress = circleRef.current;
        const length = progress.getTotalLength();
        progress.style.strokeDasharray = String(length);
        progress.style.strokeDashoffset = String(length);

        const button = linkRef.current;

        window.addEventListener('scroll', handleScroll);

        return () => window.removeEventListener("scroll", handleScroll);
    
        function handleScroll() {
            if (window.scrollY > offsetFromTop) {
                button.classList.add('show');
              } else {
                button.classList.remove('show');
              }
              const scrollPercent = (document.body.scrollTop + document.documentElement.scrollTop) / (document.documentElement.scrollHeight - document.documentElement.clientHeight);
              const draw = length * scrollPercent;
              progress.style.strokeDashoffset = String(length - draw);
        }
    }, []);

    return (
        <a
            ref={linkRef}
            className="btn-scroll-top text-decoration-none"
            href="#top"
            data-scroll
            aria-label="Scroll back to top"
            onClick={e => {
                e.preventDefault();
                window.scrollTo({top: 0, behavior: "smooth"});
            }}
        >
            <svg viewBox="0 0 40 40" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
                <circle ref={circleRef} cx="20" cy="20" r="19" fill="none" stroke="currentColor" strokeWidth="1.5" strokeMiterlimit="10"></circle>
            </svg>
            <i className="far fa-arrow-up"></i>
        </a>
    );
}