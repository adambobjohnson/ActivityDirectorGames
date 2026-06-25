import { dotnet } from './_framework/dotnet.js'
import * as keyboard from './keyboard.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

const MIN_SPLASH_MS = 5000;

function hideSplash() {
    const splash = document.querySelector('.avalonia-splash');
    if (!splash || splash.classList.contains('splash-close')) {
        return;
    }

    const video = splash.querySelector('.splash-video');
    if (video instanceof HTMLVideoElement) {
        video.pause();
    }

    splash.classList.add('splash-close');
}

function waitForMinimumSplashTime(minimumMs) {
    return new Promise((resolve) => {
        setTimeout(resolve, minimumMs);
    });
}

function waitForSplashVideoOnce() {
    const splash = document.querySelector('.avalonia-splash');
    const video = splash?.querySelector('.splash-video');

    return new Promise((resolve) => {
        if (!(video instanceof HTMLVideoElement)) {
            resolve();
            return;
        }

        const complete = () => resolve();

        if (video.ended) {
            complete();
            return;
        }

        video.addEventListener('ended', complete, { once: true });
        video.addEventListener('error', complete, { once: true });
    });
}

async function waitForSplashPlayback() {
    await Promise.all([
        waitForMinimumSplashTime(MIN_SPLASH_MS),
        waitForSplashVideoOnce(),
    ]);
}

const dotnetRuntime = await dotnet
    .withDiagnosticTracing(false)
    .withApplicationArgumentsFromQuery()
    .create();

const config = dotnetRuntime.getConfig();

dotnetRuntime.setModuleImports("main.js", {
    window: {
        location: {
            href: () => globalThis.window.location.href
        }
    },
});

keyboard.initialize(dotnetRuntime);

const mainAssemblyName = config.mainAssemblyName;
if (!mainAssemblyName)
    throw new Error("mainAssemblyName is undefined");

try {
    const appReady = dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
    await Promise.all([appReady, waitForSplashPlayback()]);
    requestAnimationFrame(() => hideSplash());
} catch (error) {
    hideSplash();
    throw error;
}
