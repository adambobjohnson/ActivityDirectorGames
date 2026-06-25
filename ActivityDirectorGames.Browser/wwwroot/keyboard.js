/// @ts-check
'use strict';

/**
 * @typedef {import("./_framework/dotnet.js").RuntimeAPI} RuntimeAPI
 */

export function initialize(/** @type {RuntimeAPI} */ dotnetRuntime) {
    (function () {
        // DEBUG: uncomment the return; to check the cursor positioning inside the input element.
        return;

        /** @type {ReturnType<setInterval> | undefined} */
        var makeInputElementVisibleId;
        function makeInputElementVisible() {
            const inputElement = document.querySelector("input.avalonia-input-element");
            if (inputElement instanceof HTMLInputElement) {
                if (inputElement.style.color === "transparent") {
                    inputElement.style.color = "black";
                    inputElement.style.zIndex = "initial";
                    clearInterval(makeInputElementVisibleId);
                }
            }
        }
        makeInputElementVisibleId = setInterval(makeInputElementVisible, 100);
    })();

    (function () {
        // Set touch-action back to 'none' while scrolling in full-screen mode, otherwise set it initially.
        // At 'initial': Viewport can be scrolled
        // At 'none': Scrolling is allowed within Avalonia
        // From 'none', you can only get back using the keyboard or system buttons.
        function updateTouchAction() {
            const out = document.getElementById("out");
            //console.log(window.visualViewport.offsetTop, window.visualViewport.pageTop);
            if (out) {
                const vpHeight = Math.floor(window.visualViewport?.height ?? 0);
                const bcHeight = Math.floor(document.body.clientHeight) - 5;
                if (vpHeight < bcHeight) {
                    out.classList.remove("fullview");
                } else {
                    if (!out.classList.contains("fullview")) {
                        out.classList.add("fullview");
                    }
                }
            }
        }
        setInterval(updateTouchAction, 300);
    })();

    (function () {
        const charsToKeyEvents = (() => {
            function usEnglishKeybindings(/** @type {String} */ text) {
                /**
                 * @param {string} c
                 * @typedef {{code: string, key: string, shiftKey?: boolean, ctrlKey?: boolean, altKey?: boolean, metaKey?: boolean}} UpDownEventData
                 * @typedef {Record<string, UpDownEventData>} ActiveModifiers
                 * @returns {{activeModifiers: ActiveModifiers, downEvent: UpDownEventData, upEvent: UpDownEventData} | undefined}
                 */
                function getKeyEventDataForCharacter(c) {
                    switch (c) {
                        // Number row
                        case "1": return { "activeModifiers": {}, "downEvent": { "code": "Digit1", "key": "1" }, "upEvent": { "code": "Digit1", "key": "1" } };
                        case "2": return { "activeModifiers": {}, "downEvent": { "code": "Digit2", "key": "2" }, "upEvent": { "code": "Digit2", "key": "2" } };
                        case "3": return { "activeModifiers": {}, "downEvent": { "code": "Digit3", "key": "3" }, "upEvent": { "code": "Digit3", "key": "3" } };
                        case "4": return { "activeModifiers": {}, "downEvent": { "code": "Digit4", "key": "4" }, "upEvent": { "code": "Digit4", "key": "4" } };
                        case "5": return { "activeModifiers": {}, "downEvent": { "code": "Digit5", "key": "5" }, "upEvent": { "code": "Digit5", "key": "5" } };
                        case "6": return { "activeModifiers": {}, "downEvent": { "code": "Digit6", "key": "6" }, "upEvent": { "code": "Digit6", "key": "6" } };
                        case "7": return { "activeModifiers": {}, "downEvent": { "code": "Digit7", "key": "7" }, "upEvent": { "code": "Digit7", "key": "7" } };
                        case "8": return { "activeModifiers": {}, "downEvent": { "code": "Digit8", "key": "8" }, "upEvent": { "code": "Digit8", "key": "8" } };
                        case "9": return { "activeModifiers": {}, "downEvent": { "code": "Digit9", "key": "9" }, "upEvent": { "code": "Digit9", "key": "9" } };
                        case "0": return { "activeModifiers": {}, "downEvent": { "code": "Digit0", "key": "0" }, "upEvent": { "code": "Digit0", "key": "0" } };
                        case "-": return { "activeModifiers": {}, "downEvent": { "code": "Minus", "key": "-" }, "upEvent": { "code": "Minus", "key": "-" } };
                        case "=": return { "activeModifiers": {}, "downEvent": { "code": "Equal", "key": "=" }, "upEvent": { "code": "Equal", "key": "=" } };

                        case "!": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit1", "key": "!", "shiftKey": true }, "upEvent": { "code": "Digit1", "key": "!", "shiftKey": true } };
                        case "@": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit2", "key": "@", "shiftKey": true }, "upEvent": { "code": "Digit2", "key": "@", "shiftKey": true } };
                        case "#": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit3", "key": "#", "shiftKey": true }, "upEvent": { "code": "Digit3", "key": "#", "shiftKey": true } };
                        case "$": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit4", "key": "$", "shiftKey": true }, "upEvent": { "code": "Digit4", "key": "$", "shiftKey": true } };
                        case "%": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit5", "key": "%", "shiftKey": true }, "upEvent": { "code": "Digit5", "key": "%", "shiftKey": true } };
                        case "^": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit6", "key": "^", "shiftKey": true }, "upEvent": { "code": "Digit6", "key": "^", "shiftKey": true } };
                        case "&": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit7", "key": "&", "shiftKey": true }, "upEvent": { "code": "Digit7", "key": "&", "shiftKey": true } };
                        case "*": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit8", "key": "*", "shiftKey": true }, "upEvent": { "code": "Digit8", "key": "*", "shiftKey": true } };
                        case "(": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit9", "key": "(", "shiftKey": true }, "upEvent": { "code": "Digit9", "key": "(", "shiftKey": true } };
                        case ")": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Digit0", "key": ")", "shiftKey": true }, "upEvent": { "code": "Digit0", "key": ")", "shiftKey": true } };
                        case "_": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Minus", "key": "_", "shiftKey": true }, "upEvent": { "code": "Minus", "key": "_", "shiftKey": true } };
                        case "+": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Equal", "key": "+", "shiftKey": true }, "upEvent": { "code": "Equal", "key": "+", "shiftKey": true } };

                        // Letter rows (QWERTY layout)
                        case "q": return { "activeModifiers": {}, "downEvent": { "code": "KeyQ", "key": "q" }, "upEvent": { "code": "KeyQ", "key": "q" } };
                        case "w": return { "activeModifiers": {}, "downEvent": { "code": "KeyW", "key": "w" }, "upEvent": { "code": "KeyW", "key": "w" } };
                        case "e": return { "activeModifiers": {}, "downEvent": { "code": "KeyE", "key": "e" }, "upEvent": { "code": "KeyE", "key": "e" } };
                        case "r": return { "activeModifiers": {}, "downEvent": { "code": "KeyR", "key": "r" }, "upEvent": { "code": "KeyR", "key": "r" } };
                        case "t": return { "activeModifiers": {}, "downEvent": { "code": "KeyT", "key": "t" }, "upEvent": { "code": "KeyT", "key": "t" } };
                        case "y": return { "activeModifiers": {}, "downEvent": { "code": "KeyY", "key": "y" }, "upEvent": { "code": "KeyY", "key": "y" } };
                        case "u": return { "activeModifiers": {}, "downEvent": { "code": "KeyU", "key": "u" }, "upEvent": { "code": "KeyU", "key": "u" } };
                        case "i": return { "activeModifiers": {}, "downEvent": { "code": "KeyI", "key": "i" }, "upEvent": { "code": "KeyI", "key": "i" } };
                        case "o": return { "activeModifiers": {}, "downEvent": { "code": "KeyO", "key": "o" }, "upEvent": { "code": "KeyO", "key": "o" } };
                        case "p": return { "activeModifiers": {}, "downEvent": { "code": "KeyP", "key": "p" }, "upEvent": { "code": "KeyP", "key": "p" } };
                        case "[": return { "activeModifiers": {}, "downEvent": { "code": "BracketLeft", "key": "[" }, "upEvent": { "code": "BracketLeft", "key": "[" } };
                        case "]": return { "activeModifiers": {}, "downEvent": { "code": "BracketRight", "key": "]" }, "upEvent": { "code": "BracketRight", "key": "]" } };
                        case "\\": return { "activeModifiers": {}, "downEvent": { "code": "Backslash", "key": "\\" }, "upEvent": { "code": "Backslash", "key": "\\" } };

                        case "a": return { "activeModifiers": {}, "downEvent": { "code": "KeyA", "key": "a" }, "upEvent": { "code": "KeyA", "key": "a" } };
                        case "s": return { "activeModifiers": {}, "downEvent": { "code": "KeyS", "key": "s" }, "upEvent": { "code": "KeyS", "key": "s" } };
                        case "d": return { "activeModifiers": {}, "downEvent": { "code": "KeyD", "key": "d" }, "upEvent": { "code": "KeyD", "key": "d" } };
                        case "f": return { "activeModifiers": {}, "downEvent": { "code": "KeyF", "key": "f" }, "upEvent": { "code": "KeyF", "key": "f" } };
                        case "g": return { "activeModifiers": {}, "downEvent": { "code": "KeyG", "key": "g" }, "upEvent": { "code": "KeyG", "key": "g" } };
                        case "h": return { "activeModifiers": {}, "downEvent": { "code": "KeyH", "key": "h" }, "upEvent": { "code": "KeyH", "key": "h" } };
                        case "j": return { "activeModifiers": {}, "downEvent": { "code": "KeyJ", "key": "j" }, "upEvent": { "code": "KeyJ", "key": "j" } };
                        case "k": return { "activeModifiers": {}, "downEvent": { "code": "KeyK", "key": "k" }, "upEvent": { "code": "KeyK", "key": "k" } };
                        case "l": return { "activeModifiers": {}, "downEvent": { "code": "KeyL", "key": "l" }, "upEvent": { "code": "KeyL", "key": "l" } };
                        case ";": return { "activeModifiers": {}, "downEvent": { "code": "Semicolon", "key": ";" }, "upEvent": { "code": "Semicolon", "key": ";" } };
                        case "'": return { "activeModifiers": {}, "downEvent": { "code": "Quote", "key": "'" }, "upEvent": { "code": "Quote", "key": "'" } };

                        case "z": return { "activeModifiers": {}, "downEvent": { "code": "KeyZ", "key": "z" }, "upEvent": { "code": "KeyZ", "key": "z" } };
                        case "x": return { "activeModifiers": {}, "downEvent": { "code": "KeyX", "key": "x" }, "upEvent": { "code": "KeyX", "key": "x" } };
                        case "c": return { "activeModifiers": {}, "downEvent": { "code": "KeyC", "key": "c" }, "upEvent": { "code": "KeyC", "key": "c" } };
                        case "v": return { "activeModifiers": {}, "downEvent": { "code": "KeyV", "key": "v" }, "upEvent": { "code": "KeyV", "key": "v" } };
                        case "b": return { "activeModifiers": {}, "downEvent": { "code": "KeyB", "key": "b" }, "upEvent": { "code": "KeyB", "key": "b" } };
                        case "n": return { "activeModifiers": {}, "downEvent": { "code": "KeyN", "key": "n" }, "upEvent": { "code": "KeyN", "key": "n" } };
                        case "m": return { "activeModifiers": {}, "downEvent": { "code": "KeyM", "key": "m" }, "upEvent": { "code": "KeyM", "key": "m" } };
                        case ",": return { "activeModifiers": {}, "downEvent": { "code": "Comma", "key": "," }, "upEvent": { "code": "Comma", "key": "," } };
                        case ".": return { "activeModifiers": {}, "downEvent": { "code": "Period", "key": "." }, "upEvent": { "code": "Period", "key": "." } };
                        case "/": return { "activeModifiers": {}, "downEvent": { "code": "Slash", "key": "/" }, "upEvent": { "code": "Slash", "key": "/" } };

                        // Shifted versions
                        case "Q": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyQ", "key": "Q", "shiftKey": true }, "upEvent": { "code": "KeyQ", "key": "Q", "shiftKey": true } };
                        case "W": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyW", "key": "W", "shiftKey": true }, "upEvent": { "code": "KeyW", "key": "W", "shiftKey": true } };
                        case "E": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyE", "key": "E", "shiftKey": true }, "upEvent": { "code": "KeyE", "key": "E", "shiftKey": true } };
                        case "R": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyR", "key": "R", "shiftKey": true }, "upEvent": { "code": "KeyR", "key": "R", "shiftKey": true } };
                        case "T": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyT", "key": "T", "shiftKey": true }, "upEvent": { "code": "KeyT", "key": "T", "shiftKey": true } };
                        case "Y": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyY", "key": "Y", "shiftKey": true }, "upEvent": { "code": "KeyY", "key": "Y", "shiftKey": true } };
                        case "U": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyU", "key": "U", "shiftKey": true }, "upEvent": { "code": "KeyU", "key": "U", "shiftKey": true } };
                        case "I": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyI", "key": "I", "shiftKey": true }, "upEvent": { "code": "KeyI", "key": "I", "shiftKey": true } };
                        case "O": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyO", "key": "O", "shiftKey": true }, "upEvent": { "code": "KeyO", "key": "O", "shiftKey": true } };
                        case "P": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyP", "key": "P", "shiftKey": true }, "upEvent": { "code": "KeyP", "key": "P", "shiftKey": true } };
                        case "{": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "BracketLeft", "key": "{", "shiftKey": true }, "upEvent": { "code": "BracketLeft", "key": "{", "shiftKey": true } };
                        case "}": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "BracketRight", "key": "}", "shiftKey": true }, "upEvent": { "code": "BracketRight", "key": "}", "shiftKey": true } };
                        case "|": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Backslash", "key": "|", "shiftKey": true }, "upEvent": { "code": "Backslash", "key": "|", "shiftKey": true } };

                        case "A": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyA", "key": "A", "shiftKey": true }, "upEvent": { "code": "KeyA", "key": "A", "shiftKey": true } };
                        case "S": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyS", "key": "S", "shiftKey": true }, "upEvent": { "code": "KeyS", "key": "S", "shiftKey": true } };
                        case "D": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyD", "key": "D", "shiftKey": true }, "upEvent": { "code": "KeyD", "key": "D", "shiftKey": true } };
                        case "F": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyF", "key": "F", "shiftKey": true }, "upEvent": { "code": "KeyF", "key": "F", "shiftKey": true } };
                        case "G": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyG", "key": "G", "shiftKey": true }, "upEvent": { "code": "KeyG", "key": "G", "shiftKey": true } };
                        case "H": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyH", "key": "H", "shiftKey": true }, "upEvent": { "code": "KeyH", "key": "H", "shiftKey": true } };
                        case "J": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyJ", "key": "J", "shiftKey": true }, "upEvent": { "code": "KeyJ", "key": "J", "shiftKey": true } };
                        case "K": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyK", "key": "K", "shiftKey": true }, "upEvent": { "code": "KeyK", "key": "K", "shiftKey": true } };
                        case "L": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyL", "key": "L", "shiftKey": true }, "upEvent": { "code": "KeyL", "key": "L", "shiftKey": true } };
                        case ":": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Semicolon", "key": ":", "shiftKey": true }, "upEvent": { "code": "Semicolon", "key": ":", "shiftKey": true } };
                        case "\"": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Quote", "key": "\"", "shiftKey": true }, "upEvent": { "code": "Quote", "key": "\"", "shiftKey": true } };

                        case "Z": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyZ", "key": "Z", "shiftKey": true }, "upEvent": { "code": "KeyZ", "key": "Z", "shiftKey": true } };
                        case "X": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyX", "key": "X", "shiftKey": true }, "upEvent": { "code": "KeyX", "key": "X", "shiftKey": true } };
                        case "C": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyC", "key": "C", "shiftKey": true }, "upEvent": { "code": "KeyC", "key": "C", "shiftKey": true } };
                        case "V": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyV", "key": "V", "shiftKey": true }, "upEvent": { "code": "KeyV", "key": "V", "shiftKey": true } };
                        case "B": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyB", "key": "B", "shiftKey": true }, "upEvent": { "code": "KeyB", "key": "B", "shiftKey": true } };
                        case "N": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyN", "key": "N", "shiftKey": true }, "upEvent": { "code": "KeyN", "key": "N", "shiftKey": true } };
                        case "M": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "KeyM", "key": "M", "shiftKey": true }, "upEvent": { "code": "KeyM", "key": "M", "shiftKey": true } };
                        case "<": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Comma", "key": "<", "shiftKey": true }, "upEvent": { "code": "Comma", "key": "<", "shiftKey": true } };
                        case ">": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Period", "key": ">", "shiftKey": true }, "upEvent": { "code": "Period", "key": ">", "shiftKey": true } };
                        case "?": return { "activeModifiers": { "Shift|ShiftLeft": { "code": "ShiftLeft", "key": "Shift", "shiftKey": true } }, "downEvent": { "code": "Slash", "key": "?", "shiftKey": true }, "upEvent": { "code": "Slash", "key": "?", "shiftKey": true } };

                        // Space
                        case " ": return { "activeModifiers": {}, "downEvent": { "code": "Space", "key": " " }, "upEvent": { "code": "Space", "key": " " } };
                    }
                };

                /** @type { ({ event: "keydown" | "keyup", data: UpDownEventData })[] } */
                const events = [];
                for (const c of text) {
                    const eventData = getKeyEventDataForCharacter(c);
                    if (!eventData) continue;

                    for (const modifierData of Object.values(eventData.activeModifiers)) {
                        events.push({ event: "keydown", data: modifierData });
                    }
                    events.push({ event: "keydown", data: eventData.downEvent });
                    events.push({ event: "keyup", data: eventData.upEvent });
                    for (const modifierData of Object.values(eventData.activeModifiers)) {
                        events.push({ event: "keyup", data: modifierData });
                    }
                }
                return events;
            }

            return usEnglishKeybindings;
        })();

        function registerInputToKeyEventsHandler(/** @type {HTMLInputElement} */ inputElement) {
            var transferInputToKey = false;
            var withinComposition = false;
            inputElement.addEventListener("keydown", (args) => {
                transferInputToKey = args.code === "" && args.key === "Unidentified";
            });
            inputElement.addEventListener("compositionstart", (args => {
                withinComposition = true;
            }));
            inputElement.addEventListener("compositionend", (args => {
                withinComposition = false;
            }));
            inputElement.addEventListener("input", (/** @type {Event} */ args) => {
                if (!transferInputToKey || withinComposition)
                    return;

                if (!(args instanceof InputEvent))
                    return;

                /**
                 * @param {string} eventName
                 * @param {ReturnType<charsToKeyEvents>[0]["data"]} data
                 */
                function dispatchKeyboardEvent(eventName, data) {
                    const initData = {
                        ...data,
                        ctrlKey: data.ctrlKey || false,
                        altKey: data.altKey || false,
                        shiftKey: data.shiftKey || false,
                        metaKey: data.metaKey || false,
                        bubbles: true,
                    };
                    inputElement.dispatchEvent(new KeyboardEvent(eventName, initData));
                }

                if (args.inputType === "insertText") {
                    const text = args.data;
                    if (!text) return;

                    for (const eventEntry of charsToKeyEvents(text)) {
                        dispatchKeyboardEvent(eventEntry.event, eventEntry.data);
                    }
                } else if (args.inputType === "insertCompositionText") {
                    const text = args.data;
                    if (!text) return;

                    for (const eventEntry of charsToKeyEvents(text)) {
                        dispatchKeyboardEvent(eventEntry.event, eventEntry.data);
                    }
                } else if (args.inputType === "deleteContentBackward") {
                    console.log("input: ", args);
                    dispatchKeyboardEvent("keydown", { "code": "Backspace", "key": "Backspace" });
                    dispatchKeyboardEvent("keyup", { "code": "Backspace", "key": "Backspace" });
                } else if (args.inputType === "insertLineBreak") {
                    console.log("input: ", args);
                    dispatchKeyboardEvent("keydown", { "code": "Enter", "key": "Enter" });
                    dispatchKeyboardEvent("keyup", { "code": "Enter", "key": "Enter" });
                }
            });
            inputElement.addEventListener("beforeinput", (args) => {
                if (args.inputType === "insertCompositionText") {
                    // deleteContentBackward is just sending the current start and end position to avalonia.
                    // This fixes the selection on some devices.
                    args.target?.dispatchEvent(new InputEvent("beforeinput", { ...args, "inputType": "deleteContentBackward" }));
                }
            });
        }

        /** @type {ReturnType<setInterval> | undefined} */
        var waitForMapTextInputToKeyEventsId;
        function waitForMapTextInputToKeyEvents() {
            const inputElement = document.querySelector("input.avalonia-input-element");
            if (inputElement instanceof HTMLInputElement) {
                clearInterval(waitForMapTextInputToKeyEventsId);
                registerInputToKeyEventsHandler(inputElement);
            }
        }
        waitForMapTextInputToKeyEventsId = setInterval(waitForMapTextInputToKeyEvents, 100);
    })();
}