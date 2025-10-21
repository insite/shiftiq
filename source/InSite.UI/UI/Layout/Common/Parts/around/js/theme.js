
/**
 * Around | Multipurpose Bootstrap HTML Template
 * Copyright 2023 Createx Studio
 * Theme scripts
 *
 * @author Createx Studio
 * @version 3.2.0
 */
      
(function () {
  'use strict';

  /**
   * Add solid background to fixed to top navigation bar
   */

  (() => {
    const navbar = document.querySelector('.navbar.fixed-top');
    if (navbar == null) return;
    const navbarClass = navbar.classList;
    const scrollOffset = 20;
    const navbarStuck = e => {
      if (e.currentTarget.pageYOffset > scrollOffset) {
        navbarClass.add('navbar-stuck');
      } else {
        navbarClass.remove('navbar-stuck');
      }
    };

    // On load
    window.addEventListener('load', e => {
      navbarStuck(e);
    });

    // On scroll
    window.addEventListener('scroll', e => {
      navbarStuck(e);
    });
  })();

  /**
   * Animation on scroll (AOS)
   *
   * @requires https://github.com/michalsnik/aos
   */

  (() => {
    const animationToggle = document.querySelector('[data-aos]');
    if (animationToggle === null) return;
    AOS.init(); // eslint-disable-line no-undef
  })();

  /**
   * Anchor smooth scrolling
   * @requires https://github.com/cferdinandi/smooth-scroll/
   */

  (() => {
    /* eslint-disable no-unused-vars, no-undef */
    const selector = '[data-scroll]',
      fixedHeader = '[data-scroll-header]';
      new SmoothScroll(selector, {
        speed: 800,
        speedAsDuration: true,
        offset: (anchor, toggle) => {
          return toggle.dataset.scrollOffset || 20;
        },
        header: fixedHeader,
        updateURL: false
      });
    /* eslint-enable no-unused-vars, no-undef */
  })();

  /**
   * Animate scroll to top button in/off view
   */

  (() => {
    const button = document.querySelector('.btn-scroll-top');
    const scrollOffset = 450;
    if (button == null) return;
    const offsetFromTop = parseInt(scrollOffset, 10);
    const progress = button.querySelector('svg circle');
    const length = progress.getTotalLength();
    progress.style.strokeDasharray = length;
    progress.style.strokeDashoffset = length;
    const showProgress = () => {
      const scrollPercent = (document.body.scrollTop + document.documentElement.scrollTop) / (document.documentElement.scrollHeight - document.documentElement.clientHeight);
      const draw = length * scrollPercent;
      progress.style.strokeDashoffset = length - draw;
    };
    window.addEventListener('scroll', e => {
      if (e.currentTarget.pageYOffset > offsetFromTop) {
        button.classList.add('show');
      } else {
        button.classList.remove('show');
      }
      showProgress();
    });
  })();

  /**
   * Cascading (Masonry) grid layout
   *
   * @requires https://github.com/desandro/imagesloaded
   * @requires https://github.com/Vestride/Shuffle
   */

  (() => {
    const grid = document.querySelectorAll('.masonry-grid');
    let masonry;
    if (grid === null) return;
    for (let i = 0; i < grid.length; i++) {
      /* eslint-disable no-undef */
      masonry = new Shuffle(grid[i], {
        itemSelector: '.masonry-grid-item',
        sizer: '.masonry-grid-item'
      });
      imagesLoaded(grid[i]).on('progress', () => {
        masonry.layout();
      });
      /* eslint-enable no-undef */

      // Filtering
      const filtersWrap = grid[i].closest('.masonry-filterable');
      if (filtersWrap === null) return;
      const filters = filtersWrap.querySelectorAll('.masonry-filters [data-group]');
      for (let n = 0; n < filters.length; n++) {
        filters[n].addEventListener('click', function (e) {
          const current = filtersWrap.querySelector('.masonry-filters .active');
          const target = this.dataset.group;
          if (current !== null) {
            current.classList.remove('active');
          }
          this.classList.add('active');
          masonry.filter(target);
          e.preventDefault();
        });
      }
    }
  })();

  /**
   * Toggling password visibility in password input
   */

  (() => {
    const elements = document.querySelectorAll('.password-toggle');
    for (let i = 0; i < elements.length; i++) {
      const passInput = elements[i].querySelector('.form-control');
      const passToggle = elements[i].querySelector('.password-toggle-btn');
      passToggle.addEventListener('click', e => {
        if (e.target.type !== 'checkbox') return;
        if (e.target.checked) {
          passInput.type = 'text';
        } else {
          passInput.type = 'password';
        }
      }, false);
    }
  })();

  /**
   * Interactive map
   * @requires https://github.com/Leaflet/Leaflet
   */

  (() => {
    const mapList = document.querySelectorAll('.interactive-map');
    if (mapList.length === 0) return;
    for (let i = 0; i < mapList.length; i++) {
      const mapOptions = mapList[i].dataset.mapOptions;
      let map;

      // Map options: Inline JSON data
      if (mapOptions && mapOptions !== '') {
        const mapOptionsObj = JSON.parse(mapOptions),
          mapLayer = mapOptionsObj.mapLayer || 'https://api.maptiler.com/maps/pastel/{z}/{x}/{y}.png?key=BO4zZpr0fIIoydRTOLSx',
          mapCenter = mapOptionsObj.center ? mapOptionsObj.center : [0, 0],
          mapZoom = mapOptionsObj.zoom || 1,
          scrollWheelZoom = mapOptionsObj.scrollWheelZoom === false ? false : true,
          markers = mapOptionsObj.markers;

        // Map setup
        /* eslint-disable no-undef */
        map = L.map(mapList[i], {
          scrollWheelZoom: scrollWheelZoom
        }).setView(mapCenter, mapZoom);

        // Tile layer
        L.tileLayer(mapLayer, {
          tileSize: 512,
          zoomOffset: -1,
          minZoom: 1,
          attribution: '\u003ca href="https://www.maptiler.com/copyright/" target="_blank"\u003e\u0026copy; MapTiler\u003c/a\u003e \u003ca href="https://www.openstreetmap.org/copyright" target="_blank"\u003e\u0026copy; OpenStreetMap contributors\u003c/a\u003e',
          crossOrigin: true
        }).addTo(map);

        // Markers
        if (markers) {
          for (let n = 0; n < markers.length; n++) {
            const iconUrl = markers[n].iconUrl,
              shadowUrl = markers[n].shadowUrl,
              markerIcon = L.icon({
                iconUrl: iconUrl || 'assets/img/map/marker-icon.png',
                iconSize: [30, 43],
                iconAnchor: [14, 43],
                shadowUrl: shadowUrl || 'assets/img/map/marker-shadow.png',
                shadowSize: [41, 41],
                shadowAnchor: [13, 41],
                popupAnchor: [1, -40]
              }),
              popup = markers[n].popup;
            const marker = L.marker(markers[n].position, {
              icon: markerIcon
            }).addTo(map);
            if (popup) {
              marker.bindPopup(popup);
            }
          }
        }

        // Map option: No options provided
      } else {
        map = L.map(mapList[i]).setView([0, 0], 1);
        L.tileLayer('https://api.maptiler.com/maps/pastel/{z}/{x}/{y}.png?key=BO4zZpr0fIIoydRTOLSx', {
          tileSize: 512,
          zoomOffset: -1,
          minZoom: 1,
          attribution: '\u003ca href="https://www.maptiler.com/copyright/" target="_blank"\u003e\u0026copy; MapTiler\u003c/a\u003e \u003ca href="https://www.openstreetmap.org/copyright" target="_blank"\u003e\u0026copy; OpenStreetMap contributors\u003c/a\u003e',
          crossOrigin: true
        }).addTo(map);
      }
      /* eslint-enable no-undef */
    }
  })();

  /**
   * Mouse move parallax effect
   * @requires https://github.com/wagerfield/parallax
   */

  (() => {
    const element = document.querySelectorAll('.parallax');

    /* eslint-disable no-unused-vars, no-undef */
    for (let i = 0; i < element.length; i++) {
      new Parallax(element[i]);
    }
    /* eslint-enable no-unused-vars, no-undef */
  })();

  /**
   * Content carousel with extensive options to control behaviour and appearance
   * @requires https://github.com/nolimits4web/swiper
   */

  (() => {
    // forEach function
    const forEach = (array, callback, scope) => {
      for (let i = 0; i < array.length; i++) {
        callback.call(scope, i, array[i]); // passes back stuff we need
      }
    };

    // Carousel initialisation
    const carousels = document.querySelectorAll('.swiper');
    forEach(carousels, (index, value) => {
      let options;
      if (value.dataset.swiperOptions != undefined) options = JSON.parse(value.dataset.swiperOptions);

      // Thumbnails
      if (options.thumbnails) {
        let images = options.thumbnails.images;
        options = Object.assign({}, options, {
          pagination: {
            el: options.thumbnails.el,
            clickable: true,
            bulletActiveClass: 'active',
            renderBullet: (index, className) => {
              return `<li class='swiper-thumbnail ${className}'>
              <img src='${images[index]}' alt='Thumbnail'>
            </li>`;
            }
          }
        });
      }
      const swiper = new Swiper(value, options); // eslint-disable-line no-undef

      // Controlled slider
      if (options.controlledSlider) {
        const controlledSlider = document.querySelector(options.controlledSlider);
        let controlledSliderOptions;
        if (controlledSlider.dataset.swiperOptions != undefined) controlledSliderOptions = JSON.parse(controlledSlider.dataset.swiperOptions);
        /* eslint-disable no-undef */
        const swiperControlled = new Swiper(controlledSlider, controlledSliderOptions);
        /* eslint-enable no-undef */
        swiper.controller.control = swiperControlled;
      }

      // Binded content
      if (options.bindedContent) {
        swiper.on('activeIndexChange', e => {
          const targetItem = document.querySelector(e.slides[e.activeIndex].dataset.swiperBinded);
          const previousItem = document.querySelector(e.slides[e.previousIndex].dataset.swiperBinded);
          previousItem.classList.remove('active');
          targetItem.classList.add('active');
        });
      }
    });
  })();

  /**
   * Gallery like styled lightbox component for presenting various types of media
   * @requires https://github.com/sachinchoolur/lightGallery
   */

  (() => {
    const gallery = document.querySelectorAll('.gallery');
    if (gallery.length) {
      for (let i = 0; i < gallery.length; i++) {
        /* eslint-disable no-undef */
        const thumbnails = gallery[i].dataset.thumbnails ? true : false,
          video = gallery[i].dataset.video ? true : false,
          defaultPlugins = [lgZoom, lgFullscreen],
          videoPlugin = video ? [lgVideo] : [],
          thumbnailPlugin = thumbnails ? [lgThumbnail] : [],
          plugins = [...defaultPlugins, ...videoPlugin, ...thumbnailPlugin];
        lightGallery(gallery[i], {
          selector: '.gallery-item',
          plugins: plugins,
          licenseKey: 'D4194FDD-48924833-A54AECA3-D6F8E646',
          download: false,
          autoplayVideoOnSlide: true,
          zoomFromOrigin: false,
          youtubePlayerParams: {
            modestbranding: 1,
            showinfo: 0,
            rel: 0
          },
          vimeoPlayerParams: {
            byline: 0,
            portrait: 0,
            color: '6366f1'
          }
        });
        /* eslint-enable no-undef */
      }
    }
  })();

  /**
   * Charts
   * @requires https://github.com/gionkunz/chartist-js
   */

  (() => {
    const chart = document.querySelectorAll('[data-chart]');
    if (chart.length === 0) return;

    // Line chart
    for (let i = 0; i < chart.length; i++) {
      const dataOptions = JSON.parse(chart[i].dataset.chart);
      new Chart(chart[i], dataOptions); // eslint-disable-line no-undef
    }
  })();

  /**
   * Range slider
   * @requires https://github.com/leongersen/noUiSlider
   */

  (() => {
    let rangeSliderWidget = document.querySelectorAll('.range-slider');
    for (let i = 0; i < rangeSliderWidget.length; i++) {
      let rangeSlider = rangeSliderWidget[i].querySelector('.range-slider-ui'),
        valueMinInput = rangeSliderWidget[i].querySelector('.range-slider-value-min'),
        valueMaxInput = rangeSliderWidget[i].querySelector('.range-slider-value-max');
      let options = {
        dataStartMin: parseInt(rangeSliderWidget[i].dataset.startMin, 10),
        dataStartMax: parseInt(rangeSliderWidget[i].dataset.startMax, 10),
        dataMin: parseInt(rangeSliderWidget[i].dataset.min, 10),
        dataMax: parseInt(rangeSliderWidget[i].dataset.max, 10),
        dataStep: parseInt(rangeSliderWidget[i].dataset.step, 10),
        dataPips: rangeSliderWidget[i].dataset.pips,
        dataTooltips: rangeSliderWidget[i].dataset.tooltips ? rangeSliderWidget[i].dataset.tooltips === 'true' : true,
        dataTooltipPrefix: rangeSliderWidget[i].dataset.tooltipPrefix || '',
        dataTooltipSuffix: rangeSliderWidget[i].dataset.tooltipSuffix || ''
      };
      let start = options.dataStartMax ? [options.dataStartMin, options.dataStartMax] : [options.dataStartMin],
        connect = options.dataStartMax ? true : 'lower';

      /* eslint-disable no-undef */
      noUiSlider.create(rangeSlider, {
        start: start,
        connect: connect,
        step: options.dataStep,
        pips: options.dataPips ? {
          mode: 'count',
          values: 5
        } : false,
        tooltips: options.dataTooltips,
        range: {
          min: options.dataMin,
          max: options.dataMax
        },
        format: {
          to: function (value) {
            return options.dataTooltipPrefix + parseInt(value, 10) + options.dataTooltipSuffix;
          },
          from: function (value) {
            return Number(value);
          }
        }
      });
      /* eslint-enable no-undef */

      rangeSlider.noUiSlider.on('update', (values, handle) => {
        let value = values[handle];
        value = value.replace(/\D/g, '');
        if (handle) {
          if (valueMaxInput) {
            valueMaxInput.value = Math.round(value);
          }
        } else {
          if (valueMinInput) {
            valueMinInput.value = Math.round(value);
          }
        }
      });
      if (valueMinInput) {
        valueMinInput.addEventListener('change', function () {
          rangeSlider.noUiSlider.set([this.value, null]);
        });
      }
      if (valueMaxInput) {
        valueMaxInput.addEventListener('change', function () {
          rangeSlider.noUiSlider.set([null, this.value]);
        });
      }
    }
  })();

  /**
   * Date / time picker
   * @requires https://github.com/flatpickr/flatpickr
   */

  (() => {
    const picker = document.querySelectorAll('.date-picker');
    if (picker.length === 0) return;
    for (let i = 0; i < picker.length; i++) {
      const defaults = {
        disableMobile: 'true'
      };
      let userOptions;
      if (picker[i].dataset.datepickerOptions != undefined) userOptions = JSON.parse(picker[i].dataset.datepickerOptions);

      /* eslint-disable no-undef */
      const linkedInput = picker[i].classList.contains('date-range') ? {
        plugins: [new rangePlugin({
          input: picker[i].dataset.linkedInput
        })]
      } : '{}';
      /* eslint-enable no-undef */
      const options = {
        ...defaults,
        ...linkedInput,
        ...userOptions
      };
      flatpickr(picker[i], options); // eslint-disable-line no-undef
    }
  })();

  /**
   * FullCalendar plugin initialization (Schedule)
   * @requires https://github.com/fullcalendar/fullcalendar
   */

  (() => {
    // forEach function
    const forEach = (array, callback, scope) => {
      for (let i = 0; i < array.length; i++) {
        callback.call(scope, i, array[i]); // passes back stuff we need
      }
    };

    // Calendar initialisation
    const calendars = document.querySelectorAll('.calendar');
    forEach(calendars, (index, value) => {
      let userOptions;
      if (value.dataset.calendarOptions != undefined) userOptions = JSON.parse(value.dataset.calendarOptions);
      let options = {
        themeSystem: 'bootstrap5',
        ...userOptions
      };

      /* eslint-disable no-undef */
      const calendarInstance = new FullCalendar.Calendar(value, options);
      /* eslint-enable no-undef */
      calendarInstance.render();
    });
  })();

  /**
   * Form validation
   */

  (() => {
    const selector = 'needs-validation';
    window.addEventListener('load', () => {
      // Fetch all the forms we want to apply custom Bootstrap validation styles to
      const forms = document.getElementsByClassName(selector);
      // Loop over them and prevent submission

      /* eslint-disable no-unused-vars */
      Array.prototype.filter.call(forms, form => {
        form.addEventListener('submit', e => {
          if (form.checkValidity() === false) {
            e.preventDefault();
            e.stopPropagation();
          }
          form.classList.add('was-validated');
        }, false);
      });
      /* eslint-enable no-unused-vars */
    }, false);
  })();

  /**
   * Input fields formatter
   * @requires https://github.com/nosir/cleave.js
   */

  (() => {
    const input = document.querySelectorAll('[data-format]');
    if (input.length === 0) return;
    for (let i = 0; i < input.length; i++) {
      let targetInput = input[i],
        cardIcon = targetInput.parentNode.querySelector('.credit-card-icon'),
        options;
      if (targetInput.dataset.format != undefined) options = JSON.parse(targetInput.dataset.format);

      /* eslint-disable no-unused-vars, no-undef */
      if (cardIcon) {
        new Cleave(targetInput, {
          ...options,
          onCreditCardTypeChanged: type => {
            cardIcon.className = 'credit-card-icon ' + type;
          }
        });
      } else {
        new Cleave(targetInput, options);
      }
      /* eslint-enable no-unused-vars, no-undef */
    }
  })();

  /**
   * Update the text of the label when radio button / checkbox changes
   */

  (() => {
    const toggleBtns = document.querySelectorAll('[data-binded-label]');
    for (let i = 0; i < toggleBtns.length; i++) {
      toggleBtns[i].addEventListener('change', function () {
        const target = this.dataset.bindedLabel;
        try {
          document.getElementById(target).textContent = this.value;
        } catch (err) {
          /* eslint-disable no-constant-condition */
          if (err.message = "Cannot set property 'textContent' of null") {
            console.error('Make sure the [data-binded-label] matches with the id of the target element you want to change text of!');
          }
          /* eslint-enable no-constant-condition */
        }
      });
    }
  })();

  /**
   * Bind different content to different navs or even accordion.
   */

  (() => {
    const clickToggles = document.querySelectorAll('[data-binded-content]');

    // Get target element siblings
    const getSiblings = elem => {
      let siblings = [],
        sibling = elem.parentNode.firstChild;
      while (sibling) {
        if (sibling.nodeType === 1 && sibling !== elem) {
          siblings.push(sibling);
        }
        sibling = sibling.nextSibling;
      }
      return siblings;
    };

    // Change binded content function
    const changeBindedContent = target => {
      const targetEl = document.querySelector(target);
      const targetSiblings = getSiblings(targetEl);
      targetSiblings.map(sibling => {
        sibling.classList.remove('active');
      });
      targetEl.classList.add('active');
    };

    // Change binded content on click
    for (let i = 0; i < clickToggles.length; i++) {
      clickToggles[i].addEventListener('click', e => {
        changeBindedContent(e.currentTarget.dataset.bindedContent);
      });
    }
  })();

  /**
   * Count input with increment (+) and decrement (-) buttons
   */

  (() => {
    const countInputs = document.querySelectorAll('.count-input');
    for (let i = 0; i < countInputs.length; i++) {
      const component = countInputs[i];
      const incrementBtn = component.querySelector('[data-increment]');
      const decrementBtn = component.querySelector('[data-decrement]');
      const input = component.querySelector('.form-control');
      const handleIncrement = () => {
        input.value++;
      };
      const handleDecrement = () => {
        if (input.value > 0) {
          input.value--;
        }
      };

      // Add click event to buttons
      incrementBtn.addEventListener('click', handleIncrement);
      decrementBtn.addEventListener('click', handleDecrement);
    }
  })();

  /**
   * Focus first input on modal / offcanvas / collapse open
   *
   */

  (() => {
    const targetInput = document.querySelectorAll('[data-focus-on-open]');
    if (targetInput === null) return;
    for (let i = 0; i < targetInput.length; i++) {
      const toggler = JSON.parse(targetInput[i].dataset.focusOnOpen);
      document.querySelector(toggler[1]).addEventListener(`shown.bs.${toggler[0]}`, () => {
        targetInput[i].focus();
      });
    }
  })();

  /**
   * Tooltip
   * @requires https://getbootstrap.com
   * @requires https://popper.js.org/
   */

  (() => {
    const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]');

    /* eslint-disable no-unused-vars, no-undef */
    [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl, {
      trigger: 'hover'
    }));
    /* eslint-enable no-unused-vars, no-undef */
  })();

  /**
   * Popover
   * @requires https://getbootstrap.com
   * @requires https://popper.js.org/
   */

  (() => {
    const popoverTriggerList = document.querySelectorAll('[data-bs-toggle="popover"]');

    /* eslint-disable no-unused-vars, no-undef */
    [...popoverTriggerList].map(popoverTriggerEl => new bootstrap.Popover(popoverTriggerEl));
    /* eslint-enable no-unused-vars, no-undef */
  })();

  /**
   * Toast
   * @requires https://getbootstrap.com
   */

  (() => {
    const toastElList = [].slice.call(document.querySelectorAll('.toast'));

    /* eslint-disable no-unused-vars, no-undef */
    toastElList.map(toastEl => new bootstrap.Toast(toastEl));
    /* eslint-enable no-unused-vars, no-undef */
  })();

  /**
   * Open YouTube video in lightbox
   * @requires https://github.com/sachinchoolur/lightGallery
   */

  (() => {
    const button = document.querySelectorAll('[data-bs-toggle="video"]');
    if (button.length) {
      for (let i = 0; i < button.length; i++) {
        /* eslint-disable no-undef */
        lightGallery(button[i], {
          selector: 'this',
          plugins: [lgVideo],
          licenseKey: 'D4194FDD-48924833-A54AECA3-D6F8E646',
          download: false,
          youtubePlayerParams: {
            modestbranding: 1,
            showinfo: 0,
            rel: 0
          },
          vimeoPlayerParams: {
            byline: 0,
            portrait: 0,
            color: '6366f1'
          }
        });
        /* eslint-enable no-undef */
      }
    }
  })();

  /**
   * Price switch
   */

  (() => {
    const switchWrapper = document.querySelectorAll('.price-switch-wrapper');
    if (switchWrapper.length <= 0) return;
    const showMonthlyPrice = (monthlyPrice, annualPrice) => {
      for (let n = 0; n < monthlyPrice.length; n++) {
        annualPrice[n].classList.add('d-none');
        monthlyPrice[n].classList.remove('d-none');
      }
    };
    const showAnnualPrice = (monthlyPrice, annualPrice) => {
      for (let n = 0; n < monthlyPrice.length; n++) {
        monthlyPrice[n].classList.add('d-none');
        annualPrice[n].classList.remove('d-none');
      }
    };
    for (let i = 0; i < switchWrapper.length; i++) {
      const switchToggle = switchWrapper[i].querySelector('[data-bs-toggle="price"]');
      switchToggle.addEventListener('change', e => {
        const monthlySwitch = e.currentTarget.querySelector('[data-monthly-switch]');
        const annualSwitch = e.currentTarget.querySelector('[data-annual-switch]');
        const monthlyPrice = e.currentTarget.closest('.price-switch-wrapper').querySelectorAll('[data-monthly-price]');
        const annualPrice = e.currentTarget.closest('.price-switch-wrapper').querySelectorAll('[data-annual-price]');
        if (monthlySwitch.checked == true) showMonthlyPrice(monthlyPrice, annualPrice);
        if (annualSwitch.checked == true) showAnnualPrice(monthlyPrice, annualPrice);
      });
    }
  })();

  /**
   * Toggle that checkes / unchecks all target checkboxes at once
   */

  (() => {
    const toggler = document.querySelectorAll('[data-bs-toggle="checkbox"]');
    if (toggler.length === 0) return;
    for (let i = 0; i < toggler.length; i++) {
      toggler[i].addEventListener('click', e => {
        e.preventDefault();
        let checkboxListContainer = document.querySelector(e.target.dataset.bsTarget),
          checkboxList = checkboxListContainer.querySelectorAll('input[type="checkbox"]');
        checkboxListContainer.classList.toggle('all-checked');
        if (checkboxListContainer.classList.contains('all-checked')) {
          for (let n = 0; n < checkboxList.length; n++) {
            checkboxList[n].checked = true;
          }
        } else {
          for (let m = 0; m < checkboxList.length; m++) {
            checkboxList[m].checked = false;
          }
        }
      });
    }
  })();

  /**
   * Countdown timer
   * @requires https://github.com/BrooonS/timezz
   */

  (() => {
    const timers = document.querySelectorAll('.countdown');
    if (timers.length === 0) return;
    for (let i = 0; i < timers.length; i++) {
      const date = timers[i].dataset.countdownDate;

      /* eslint-disable no-undef */
      timezz(timers[i], {
        date: date
        // add more options here
      });
      /* eslint-enable no-undef */
    }
  })();

  /**
   * Ajaxify MailChimp subscription form
   */

  (() => {
    const form = document.querySelectorAll('.subscription-form');
    if (form === null) return;
    for (let i = 0; i < form.length; i++) {
      const button = form[i].querySelector('button[type="submit"]'),
        buttonText = button.innerHTML,
        input = form[i].querySelector('.form-control'),
        antispam = form[i].querySelector('.subscription-form-antispam'),
        status = form[i].querySelector('.subscription-status');
      form[i].addEventListener('submit', function (e) {
        if (e) e.preventDefault();
        if (antispam.value !== '') return;
        register(this, button, input, buttonText, status);
      });
    }
    const register = (form, button, input, buttonText, status) => {
      button.innerHTML = 'Sending...';

      // Get url for MailChimp
      const url = form.action.replace('/post?', '/post-json?');

      // Add form data to object
      const data = '&' + input.name + '=' + encodeURIComponent(input.value);

      // Create and add post script to the DOM
      const script = document.createElement('script');
      script.src = url + '&c=callback' + data;
      document.body.appendChild(script);

      // Callback function
      const callback = 'callback';
      window[callback] = response => {
        // Remove post script from the DOM
        delete window[callback];
        document.body.removeChild(script);

        // Change button text back to initial
        button.innerHTML = buttonText;

        // Display content and apply styling to response message conditionally
        if (response.result == 'success') {
          input.classList.remove('is-invalid');
          input.classList.add('is-valid');
          status.classList.remove('status-error');
          status.classList.add('status-success');
          status.innerHTML = response.msg;
          setTimeout(() => {
            input.classList.remove('is-valid');
            status.innerHTML = '';
            status.classList.remove('status-success');
          }, 6000);
        } else {
          input.classList.remove('is-valid');
          input.classList.add('is-invalid');
          status.classList.remove('status-success');
          status.classList.add('status-error');
          status.innerHTML = response.msg.substring(4);
          setTimeout(() => {
            input.classList.remove('is-invalid');
            status.innerHTML = '';
            status.classList.remove('status-error');
          }, 6000);
        }
      };
    };
  })();

})();
