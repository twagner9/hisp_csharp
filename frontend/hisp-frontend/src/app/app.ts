import { Component, signal } from '@angular/core';
import { EditableText } from './EditableText';
import { UploadButton } from './UploadButton';

@Component({
  selector: 'app-root',
  imports: [EditableText, UploadButton],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('hisp-frontend');
}
