import { Component, signal } from '@angular/core';
import { UserNameService } from '../../core/services/user-name.service';
import { EditableText } from './editable-text';

@Component({
  selector: 'name-text',
  imports: [EditableText],
  template: `<h1>
    Hello <editable-text [startingText]="'Anonymous'"></editable-text>, ready to process images?
  </h1>`,
})
export class NameText {
  constructor(public svc: UserNameService) {}
}
