import { dotnet } from './_framework/dotnet.js'
import * as keyboard from './keyboard.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

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
    await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
    requestAnimationFrame(() => hideSplash());
} catch (error) {
    hideSplash();
    throw error;
}
