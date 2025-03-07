import { dotnet } from './_framework/dotnet.js'
import * as keyboard from './keyboard.js'

const is_browser = typeof window != "undefined";
if (!is_browser) throw new Error(`Expected to be running in a browser`);

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

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
