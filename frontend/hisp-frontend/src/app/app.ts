import { Component, signal } from '@angular/core';
import { UploadButton } from './features/image-upload/UploadButton';
import { NameText } from './features/user-info/NameText';
import { ProcessSelection } from './features/process-selection/ProcessSelection';
import { SubmitButton } from './features/submit/SubmitButton';
import { ImageDisplay } from './ImageDisplay';

@Component({
  selector: 'app-root',
  imports: [UploadButton, NameText, ProcessSelection, SubmitButton, ImageDisplay],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('hisp-frontend');
}
