/* MTM Inventory Application - Modern Documentation JavaScript */

(function() {
    'use strict';

    // Utility functions
    const $ = (selector) => document.querySelector(selector);
    const $$ = (selector) => document.querySelectorAll(selector);

    // Initialize when DOM is ready
    document.addEventListener('DOMContentLoaded', function() {
        initializeSidebar();
        initializeSearch();
        initializeFAQ();
        initializeThemeToggle();
        initializeLanguageToggle();
        initializeMobileMenu();
        initializeProgressTracking();
        initializeAccessibility();
        initializeScrollEffects();
    });

    // Sidebar navigation
    function initializeSidebar() {
        const sidebarLinks = $$('.sidebar nav a');
        const currentPath = window.location.pathname;

        sidebarLinks.forEach(link => {
            if (link.getAttribute('href') === currentPath.split('/').pop()) {
                link.classList.add('active');
            }

            link.addEventListener('click', function(e) {
                sidebarLinks.forEach(l => l.classList.remove('active'));
                this.classList.add('active');
            });
        });
    }

    // Search functionality
    function initializeSearch() {
        const searchInput = $('.search-input');
        if (!searchInput) return;

        let searchTimeout;
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            searchTimeout = setTimeout(() => {
                performSearch(this.value);
            }, 300);
        });

        // Keyboard navigation for search
        searchInput.addEventListener('keydown', function(e) {
            if (e.key === 'Escape') {
                this.value = '';
                clearSearchResults();
            }
        });
    }

    function performSearch(query) {
        if (!query || query.length < 2) {
            clearSearchResults();
            return;
        }

        // Simple content search
        const searchableElements = $$('h1, h2, h3, h4, h5, h6, p, li');
        const results = [];

        searchableElements.forEach(element => {
            if (element.textContent.toLowerCase().includes(query.toLowerCase())) {
                results.push({
                    element: element,
                    text: element.textContent,
                    type: element.tagName.toLowerCase()
                });
            }
        });

        displaySearchResults(results, query);
    }

    function displaySearchResults(results, query) {
        let searchResults = $('.search-results');
        if (!searchResults) {
            searchResults = document.createElement('div');
            searchResults.className = 'search-results';
            $('.search-container').appendChild(searchResults);
        }

        if (results.length === 0) {
            searchResults.innerHTML = `<p class="text-muted">No results found for "${query}"</p>`;
            return;
        }

        const resultsHTML = results.slice(0, 10).map(result => {
            const highlightedText = highlightSearchTerm(result.text, query);
            return `
                <div class="search-result-item" data-type="${result.type}">
                    <div class="search-result-text">${highlightedText}</div>
                </div>
            `;
        }).join('');

        searchResults.innerHTML = `
            <div class="search-results-header">
                <strong>${results.length} result${results.length !== 1 ? 's' : ''} found</strong>
            </div>
            ${resultsHTML}
        `;

        // Add click handlers for search results
        $$('.search-result-item').forEach(item => {
            item.addEventListener('click', function() {
                const targetElement = results[Array.from(this.parentNode.children).indexOf(this) - 1].element;
                scrollToElement(targetElement);
                clearSearchResults();
            });
        });
    }

    function highlightSearchTerm(text, query) {
        const regex = new RegExp(`(${query})`, 'gi');
        return text.replace(regex, '<mark>$1</mark>');
    }

    function clearSearchResults() {
        const searchResults = $('.search-results');
        if (searchResults) {
            searchResults.remove();
        }
    }

    function scrollToElement(element) {
        element.scrollIntoView({
            behavior: 'smooth',
            block: 'start'
        });
        
        // Highlight the target element briefly
        element.style.backgroundColor = 'rgba(75, 69, 237, 0.2)';
        setTimeout(() => {
            element.style.backgroundColor = '';
        }, 2000);
    }

    // FAQ accordion functionality
    function initializeFAQ() {
        $$('.faq-question').forEach(question => {
            question.addEventListener('click', function() {
                const answer = this.nextElementSibling;
                const isActive = this.classList.contains('active');

                // Close all other FAQ items
                $$('.faq-question').forEach(q => {
                    q.classList.remove('active');
                    q.nextElementSibling.classList.remove('active');
                });

                // Toggle current item
                if (!isActive) {
                    this.classList.add('active');
                    answer.classList.add('active');
                }
            });
        });
    }

    // Theme toggle (dark/light mode)
    function initializeThemeToggle() {
        const themeToggle = $('.theme-toggle');
        if (!themeToggle) return;

        const currentTheme = localStorage.getItem('theme') || 'light';
        document.documentElement.setAttribute('data-theme', currentTheme);

        themeToggle.addEventListener('click', function() {
            const newTheme = document.documentElement.getAttribute('data-theme') === 'dark' ? 'light' : 'dark';
            document.documentElement.setAttribute('data-theme', newTheme);
            localStorage.setItem('theme', newTheme);
        });
    }

    // Language toggle (Technical/Plain English)
    function initializeLanguageToggle() {
        const languageToggle = $('.language-toggle');
        if (!languageToggle) return;

        languageToggle.addEventListener('click', function() {
            const currentMode = this.getAttribute('data-mode') || 'technical';
            const newMode = currentMode === 'technical' ? 'plain' : 'technical';
            
            if (newMode === 'plain') {
                switchToPlainEnglish();
                this.textContent = 'Switch to Technical';
            } else {
                switchToTechnical();
                this.textContent = 'Plain English';
            }
            
            this.setAttribute('data-mode', newMode);
        });
    }

    function switchToPlainEnglish() {
        document.body.classList.add('plain-english');
        
        // Store preference
        localStorage.setItem('language-mode', 'plain');
        
        // You could also redirect to plain English versions
        const plainEnglishUrl = window.location.pathname.replace('/Technical/', '/PlainEnglish/');
        if (plainEnglishUrl !== window.location.pathname) {
            // Check if plain English version exists
            fetch(plainEnglishUrl, { method: 'HEAD' })
                .then(response => {
                    if (response.ok) {
                        window.location.href = plainEnglishUrl;
                    }
                })
                .catch(() => {
                    // Fallback to CSS-only plain English mode
                    console.log('Plain English version not found, using CSS mode');
                });
        }
    }

    function switchToTechnical() {
        document.body.classList.remove('plain-english');
        localStorage.setItem('language-mode', 'technical');
        
        const technicalUrl = window.location.pathname.replace('/PlainEnglish/', '/Technical/');
        if (technicalUrl !== window.location.pathname) {
            window.location.href = technicalUrl;
        }
    }

    // Mobile menu functionality
    function initializeMobileMenu() {
        const mobileMenuToggle = $('.mobile-menu-toggle');
        const sidebar = $('.sidebar');
        
        if (!mobileMenuToggle || !sidebar) return;

        mobileMenuToggle.addEventListener('click', function() {
            sidebar.classList.toggle('open');
            this.setAttribute('aria-expanded', sidebar.classList.contains('open'));
        });

        // Close sidebar when clicking outside on mobile
        document.addEventListener('click', function(e) {
            if (window.innerWidth <= 768 && 
                !sidebar.contains(e.target) && 
                !mobileMenuToggle.contains(e.target)) {
                sidebar.classList.remove('open');
                mobileMenuToggle.setAttribute('aria-expanded', 'false');
            }
        });
    }

    // Progress tracking for multi-page documentation
    function initializeProgressTracking() {
        const progressBar = $('.progress-bar');
        if (!progressBar) return;

        // Calculate reading progress based on scroll
        window.addEventListener('scroll', function() {
            const winScroll = document.body.scrollTop || document.documentElement.scrollTop;
            const height = document.documentElement.scrollHeight - document.documentElement.clientHeight;
            const scrolled = (winScroll / height) * 100;
            
            progressBar.style.width = scrolled + '%';
        });
    }

    // Accessibility enhancements
    function initializeAccessibility() {
        // Skip to main content
        const skipLink = $('.skip-link');
        if (skipLink) {
            skipLink.addEventListener('click', function(e) {
                e.preventDefault();
                const mainContent = $('.main-content');
                if (mainContent) {
                    mainContent.focus();
                    mainContent.scrollIntoView();
                }
            });
        }

        // Keyboard navigation for cards
        $$('.card, .mtm-card').forEach(card => {
            if (!card.getAttribute('tabindex')) {
                card.setAttribute('tabindex', '0');
            }
        });

        // High contrast mode detection
        if (window.matchMedia('(prefers-contrast: high)').matches) {
            document.body.classList.add('high-contrast');
        }

        // Reduced motion preference
        if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) {
            document.body.classList.add('reduced-motion');
        }
    }

    // Scroll effects
    function initializeScrollEffects() {
        // Intersection Observer for fade-in animations
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver(function(entries) {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('fade-in');
                }
            });
        }, observerOptions);

        // Observe elements that should fade in
        $$('.card, .mtm-card, .concept-card').forEach(el => {
            observer.observe(el);
        });

        // Sticky header behavior
        const header = $('.header');
        if (header) {
            let lastScrollTop = 0;
            window.addEventListener('scroll', function() {
                const scrollTop = window.pageYOffset || document.documentElement.scrollTop;
                
                if (scrollTop > lastScrollTop && scrollTop > 100) {
                    header.style.transform = 'translateY(-100%)';
                } else {
                    header.style.transform = 'translateY(0)';
                }
                
                lastScrollTop = scrollTop;
            });
        }
    }

    // Utility functions for external use
    window.MTMDocs = {
        scrollTo: scrollToElement,
        search: performSearch,
        clearSearch: clearSearchResults,
        
        // Analytics helper (if needed)
        trackEvent: function(category, action, label) {
            if (typeof gtag !== 'undefined') {
                gtag('event', action, {
                    event_category: category,
                    event_label: label
                });
            }
        }
    };

    // Print functionality
    window.addEventListener('beforeprint', function() {
        // Expand all FAQ items for printing
        $$('.faq-answer').forEach(answer => {
            answer.style.display = 'block';
        });
    });

    window.addEventListener('afterprint', function() {
        // Restore FAQ state after printing
        $$('.faq-answer').forEach(answer => {
            if (!answer.classList.contains('active')) {
                answer.style.display = 'none';
            }
        });
    });

})();