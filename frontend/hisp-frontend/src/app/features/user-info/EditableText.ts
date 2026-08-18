import {
  Component,
  ElementRef,
  signal,
  effect,
  ViewChild,
  input,
  afterRenderEffect,
} from '@angular/core';
import { UserNameService } from '../../core/services/user-name.service';

@Component({
  selector: 'editable-text',
  templateUrl: 'editable-text.html',
  styleUrl: 'editable-text.css',
})

/**
 * Highly reusable component that should allow users to edit the text being clicked.
 */
export class EditableText {
  startingText = input<string>('');
  editing = signal(false);
  savedText = signal<string>(this.startingText());
  prevText = signal<string>(this.savedText());

  // ViewChild gets the ElementReference so that I actually interact with the DOM element
  @ViewChild('editableTextInput') editableTextInput!: ElementRef<HTMLInputElement>;

  // constructor is simply an early place to start this effect watcher; it will watch for
  // reads of this.editing() signal, and perform the logic as required.
  // Here, I check negation, and do nothing if it's checked, but by extension,
  // if it's true, it will queue the task of forcing the input element to have focus.
  constructor(public svc: UserNameService) {
    effect(() => {
      this.savedText.set(this.startingText());
    });

    // afterNextRender runs only once, while afterRenderEffect is tied to the signal,
    // so it reacts to the state once the signal is updated, and applies the changes
    // to DOM after the update occurred -- which is what I want in this scenario.
    afterRenderEffect(() => {
      if (!this.editing()) return;

      this.editableTextInput?.nativeElement.focus();
    });
  }

  saveUserInput() {
    if (this.savedText() === '' || this.savedText() == null) {
      this.savedText.set(this.prevText());
    }
    this.prevText.set(this.savedText());
    this.svc.text.set(this.savedText());
    this.editing.set(false);
  }
}
