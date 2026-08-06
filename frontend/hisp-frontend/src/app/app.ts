import { Component, signal } from '@angular/core';
import { EditableText } from './EditableText';
import { UploadButton } from './UploadButton';
import { NameText } from './NameText';

@Component({
  selector: 'app-root',
  imports: [UploadButton, NameText],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('hisp-frontend');
}
