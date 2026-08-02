import { Component, signal } from "@angular/core";

@Component ({
    selector: 'editable-text',
    templateUrl: 'editable-text.html'
})

/**
 * Highly reusable component that should allow users to edit the text being clicked.
 */
export class EditableText {
    editing = signal(false);
    savedText = signal("Anonymous");
    currentText = signal(this.savedText);
}