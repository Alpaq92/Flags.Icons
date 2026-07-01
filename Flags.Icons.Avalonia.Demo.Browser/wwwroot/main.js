import { dotnet } from './_framework/dotnet.js'

// disableIntegrityCheck: skip the boot-resource SRI hashes. A returning visitor can hold
// _framework files cached from a previous deploy whose bytes no longer match the new boot
// manifest's inlined SHA-256 — which the loader otherwise rejects, leaving the demo stuck on
// the splash. GitHub Pages gives no Cache-Control header control to flush that cache, so we
// drop the integrity gate instead. Safe for a public, read-only demo.
const dotnetRuntime = await dotnet
    .withConfig({ disableIntegrityCheck: true })
    .withDiagnosticTracing(false)
    .create();

const config = dotnetRuntime.getConfig();

// Fade the loading splash out once the runtime is ready to render, then remove it.
const splash = document.getElementById('loading');
if (splash) {
    splash.classList.add('hide');
    splash.addEventListener('transitionend', () => splash.remove(), { once: true });
    setTimeout(() => splash.remove(), 700);   // fallback if the transition doesn't fire
}

await dotnetRuntime.runMain(config.mainAssemblyName, [globalThis.location.href]);
