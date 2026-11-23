async ({ playMusic, showSnowmen = false, showChristmasBanner = false, specialFlakes = false, showFlakeToggle = false }) => {
    if (document.getElementById('snow-canvas')) {
        return;
    }

    const canvas = document.createElement('canvas');
    canvas.id = 'snow-canvas';
    canvas.setAttribute('aria-hidden', 'true');
    Object.assign(canvas.style, {
        position: 'fixed',
        left: '0',
        top: '0',
        width: '100%',
        height: '100%',
        pointerEvents: 'none',
        zIndex: '9999'
    });
    document.body.appendChild(canvas);

    const context = canvas.getContext('2d');
    if (!context) throw new Error('The Christmas demo requires a 2D canvas context');

    const resize = () => {
        canvas.width = window.innerWidth;
        canvas.height = window.innerHeight;
    };
    resize();
    window.addEventListener('resize', resize);

    if (showFlakeToggle) {
        const toggle = document.createElement('button');
        toggle.id = 'snowflake-toggle';
        toggle.type = 'button';
        toggle.setAttribute('aria-label', 'Spezialflocken (C#, .NET, DDC)');
        toggle.setAttribute('aria-controls', canvas.id);
        Object.assign(toggle.style, {
            position: 'fixed', top: '16px', right: '16px', zIndex: '10002',
            maxWidth: 'calc(100vw - 32px)', minHeight: '44px', padding: '10px 16px',
            border: '2px solid #e8c76b', borderRadius: '8px', color: '#fff8dc',
            font: 'bold 14px system-ui, sans-serif', cursor: 'pointer',
            pointerEvents: 'auto', boxShadow: '0 4px 12px rgba(0, 0, 0, 0.35)'
        });
        const updateToggle = () => {
            toggle.setAttribute('aria-pressed', String(specialFlakes));
            toggle.textContent = specialFlakes ? 'Spezialflocken: AN' : 'Spezialflocken: AUS';
            toggle.style.background = specialFlakes ? '#512b81' : '#155b38';
        };
        toggle.addEventListener('click', () => {
            specialFlakes = !specialFlakes;
            updateToggle();
        });
        updateToggle();
        document.body.appendChild(toggle);
    }

    const symbols = ['C#', '.NET', 'DDC','Köln','Xebia'];
    const flakes = [];
    for (let i = 0; i < 120; i += 1) {
        flakes.push({
            x: Math.random() * canvas.width,
            y: Math.random() * canvas.height,
            r: Math.random() * 6 + 3,
            d: Math.random() * 120,
            vx: (Math.random() - 0.5) * 1.5,
            vy: Math.random() + 0.5,
            color: `hsla(${Math.random() * 360}, 80%, 90%, 0.9)`,
            symbol: symbols[i % symbols.length]
        });
    }

    let angle = 0;
    const draw = () => {
        context.clearRect(0, 0, canvas.width, canvas.height);
        angle += 0.01;
        for (const flake of flakes) {
            context.fillStyle = flake.color;
            if (specialFlakes) {
                context.font = `bold ${Math.round(flake.r * 2 + 10)}px system-ui, sans-serif`;
                context.textAlign = 'center';
                context.textBaseline = 'middle';
                context.fillText(flake.symbol, flake.x, flake.y);
            } else {
                context.beginPath();
                context.arc(flake.x, flake.y, flake.r, 0, Math.PI * 2);
                context.fill();
            }

            flake.y += (Math.cos(angle + flake.d) + flake.vy + flake.r * 0.3) * (specialFlakes ? 0.75 : 1);
            flake.x += Math.sin(angle) * 0.5 + flake.vx;
            if (flake.x > canvas.width || flake.x < 0 || flake.y > canvas.height) {
                flake.x = Math.random() * canvas.width;
                flake.y = -10;
            }
        }
        requestAnimationFrame(draw);
    };
    draw();

    if (showChristmasBanner) {
        const banner = document.createElement('div');
        banner.id = 'christmas-banner';
        banner.setAttribute('role', 'status');
        Object.assign(banner.style, {
            position: 'fixed', top: '50%', left: '50%', transform: 'translate(-50%, -50%)',
            boxSizing: 'border-box', maxWidth: 'calc(100vw - 32px)',
            padding: '10px 24px', border: '2px solid #e8c76b', borderRadius: '12px',
            background: 'linear-gradient(135deg, #155b38, #8b1e2d)',
            color: '#fff8dc', fontFamily: 'Georgia, serif', fontWeight: 'bold',
            fontSize: 'clamp(18px, 3vw, 30px)', textAlign: 'center',
            boxShadow: '0 4px 18px rgba(0, 0, 0, 0.35)', pointerEvents: 'none',
            zIndex: '10001'
        });
        banner.innerHTML = '<span aria-hidden="true">&#10052; </span>Happy Christmas<span aria-hidden="true"> &#10052;</span>';
        document.body.appendChild(banner);
        banner.animate([
            { opacity: 0, transform: 'translate(-50%, -50%) scale(0.85)' },
            { opacity: 1, transform: 'translate(-50%, -50%) scale(1)' }
        ], { duration: 750, easing: 'cubic-bezier(0.2, 0.8, 0.2, 1)', fill: 'both' });
        banner.animate([
            { marginTop: '0px' },
            { marginTop: '-10px' }
        ], { duration: 2200, easing: 'ease-in-out', direction: 'alternate', iterations: Infinity });
    }

    if (showSnowmen) {
        const layer = document.createElement('div');
        layer.id = 'snowmen-layer';
        layer.setAttribute('aria-hidden', 'true');
        Object.assign(layer.style, {
            position: 'fixed', left: '0', bottom: '0', width: '100%', height: '140px',
            overflow: 'hidden', pointerEvents: 'none', zIndex: '10000'
        });
        document.body.appendChild(layer);

        const scarfColors = ['#d62828', '#267d49', '#3579bc', '#a855b5'];
        const spawnSnowman = (direction, alreadyOnScreen = false) => {
            const width = 64 + Math.random() * 24;
            const scarf = scarfColors[Math.floor(Math.random() * scarfColors.length)];
            const walker = document.createElement('div');
            walker.className = 'snowman';
            walker.dataset.direction = direction;
            Object.assign(walker.style, {
                position: 'absolute', left: '0', bottom: '4px', width: `${width}px`,
                height: `${width * 110 / 80}px`, pointerEvents: 'none'
            });
            walker.innerHTML = `
                <div style="transform: scaleX(${direction === 'left' ? -1 : 1})">
                    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 80 110"
                         style="display:block;width:100%;height:auto;overflow:visible;transform-origin:50% 100%">
                        <g class="snowman-foot"><ellipse cx="27" cy="104" rx="12" ry="5" fill="#334155"/></g>
                        <g class="snowman-foot"><ellipse cx="53" cy="104" rx="12" ry="5" fill="#334155"/></g>
                        <path d="M25 57L8 45M8 45L5 36M8 45L1 44M55 57L72 45M72 45L75 36M72 45L79 44"
                              fill="none" stroke="#795133" stroke-width="3" stroke-linecap="round"/>
                        <g fill="#f8fafc" stroke="#b8cede" stroke-width="1.5">
                            <circle cx="40" cy="79" r="24"/>
                            <circle cx="40" cy="54" r="18"/>
                            <circle cx="40" cy="29" r="14"/>
                        </g>
                        <path d="M51 29L69 33L51 36Z" fill="#f58b27"/>
                        <circle cx="45" cy="25" r="2" fill="#17212e"/>
                        <path d="M44 36Q49 40 53 36" fill="none" stroke="#17212e" stroke-width="1.5"/>
                        <path d="M25 44H55V51H25ZM46 51H54V67H46Z" fill="${scarf}"/>
                        <g fill="#17212e">
                            <rect x="26" y="5" width="28" height="17" rx="2"/>
                            <rect x="21" y="20" width="38" height="5" rx="2"/>
                            <circle cx="40" cy="58" r="2"/><circle cx="40" cy="70" r="2"/>
                            <circle cx="40" cy="83" r="2"/>
                        </g>
                        <rect x="26" y="15" width="28" height="5" fill="${scarf}"/>
                    </svg>
                </div>`;
            layer.appendChild(walker);

            const stepDuration = 550 + Math.random() * 300;
            walker.querySelector('svg').animate([
                { transform: 'translateY(0) rotate(-3deg)' },
                { transform: 'translateY(-3px) rotate(3deg)' },
                { transform: 'translateY(0) rotate(-3deg)' }
            ], { duration: stepDuration, iterations: Infinity });
            walker.querySelectorAll('.snowman-foot').forEach((foot, index) => {
                foot.animate([
                    { transform: 'translateY(0)' },
                    { transform: 'translateY(-5px)' },
                    { transform: 'translateY(0)' }
                ], { duration: stepDuration, delay: index * stepDuration / 2, iterations: Infinity });
            });

            const walk = walker.animate([
                { transform: 'translateX(-100%)' },
                { transform: 'translateX(100vw)' }
            ], {
                duration: (window.innerWidth + width) / (40 + Math.random() * 40) * 1000,
                direction: direction === 'left' ? 'reverse' : 'normal',
                fill: 'forwards'
            });
            if (alreadyOnScreen) {
                walk.currentTime = walk.effect.getTiming().duration * (0.15 + Math.random() * 0.15);
            }
            walk.onfinish = () => {
                walker.getAnimations({ subtree: true }).forEach(animation => animation.cancel());
                walker.remove();
            };
        };

        // Start with both directions visible; later arrivals are entirely random.
        const firstDirection = Math.random() < 0.5 ? 'left' : 'right';
        spawnSnowman(firstDirection, true);
        spawnSnowman(firstDirection === 'left' ? 'right' : 'left', true);
        let spawnTimer;
        const scheduleSpawn = () => {
            spawnTimer = setTimeout(() => {
                if (layer.childElementCount < 4) {
                    spawnSnowman(Math.random() < 0.5 ? 'left' : 'right');
                }
                scheduleSpawn();
            }, 2000 + Math.random() * 3000);
        };
        scheduleSpawn();
        window.addEventListener('pagehide', () => clearTimeout(spawnTimer), { once: true });
    }

    if (playMusic && !window.__jingleLoop) {
        const AudioContext = window.AudioContext || window.webkitAudioContext;
        if (!AudioContext) throw new Error('The Christmas jingle requires Web Audio');
        const audio = new AudioContext();
        await audio.resume();
        const beat = 60 / 140;
        const melody = [
            [392, 0.5], [392, 0.5], [392, 1],
            [392, 0.5], [392, 0.5], [392, 1],
            [392, 0.5], [494, 0.5], [330, 0.75], [370, 0.25], [392, 2],
            [440, 0.5], [440, 0.5], [440, 0.75], [440, 0.25], [440, 0.5],
            [392, 0.5], [392, 0.5], [392, 0.25], [392, 0.25], [392, 0.5],
            [370, 0.5], [370, 0.5], [392, 0.5], [370, 1], [494, 1]
        ];
        const loop = () => {
            let time = audio.currentTime + 0.05;
            for (const [frequency, beats] of melody) {
                const oscillator = audio.createOscillator();
                const gain = audio.createGain();
                gain.gain.setValueAtTime(0.08, time);
                gain.gain.exponentialRampToValueAtTime(0.0001, time + beats * beat);
                oscillator.frequency.value = frequency;
                oscillator.type = 'sine';
                oscillator.connect(gain).connect(audio.destination);
                oscillator.start(time);
                oscillator.stop(time + beats * beat);
                time += beats * beat;
            }
            setTimeout(loop, Math.max(0, (time - audio.currentTime) * 1000));
        };
        loop();
        window.__jingleLoop = true;
    }
}
