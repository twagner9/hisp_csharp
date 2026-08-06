import { Component, ElementRef, signal, effect, ViewChild } from "@angular/core";

@Component ({
    selector: 'editable-text',
    templateUrl: 'editable-text.html',
    styleUrl: 'editable-text.css'
})

/**
 * Highly reusable component that should allow users to edit the text being clicked.
 */
export class EditableText {
    editing = signal(false);
    savedText = signal('Anonymous');
    prevText = signal(this.savedText());

    // ViewChild gets the ElementReference so that I actually interact with the DOM element
    @ViewChild('editableTextInput') editableTextInput!: ElementRef<HTMLInputElement>;

    // constructor is simply an early place to start this effect watcher; it will watch for
    // reads of this.editing() signal, and perform the logic as required.
    // Here, I check negation, and do nothing if it's checked, but by extension,
    // if it's true, it will queue the task of forcing the input element to have focus.
    constructor() {
        effect(() => {
            if (!this.editing()) return;

            queueMicrotask(() => this.editableTextInput?.nativeElement.focus());
        })
    }

	saveUserInput() {
		if (this.savedText() === "" || this.savedText() == null) {
			this.savedText.set(this.prevText());
		}
        this.prevText.set(this.savedText());
        this.editing.set(false);
    }
}