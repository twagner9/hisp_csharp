import { Component, signal } from '@angular/core';
import { UploadButton } from './features/image-upload/upload-button';
import { NameText } from './features/user-info/name-text';
import { ProcessSelection } from './features/process-selection/process-selection';
import { SubmitButton } from './features/submit/submit-button';
import { ImageDisplay } from './features/image-display/image-display';

@Component({
  selector: 'app-root',
  imports: [UploadButton, NameText, ProcessSelection, SubmitButton, ImageDisplay],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('hisp-frontend');
}
