import { Component, signal } from '@angular/core';
import { UploadButton } from './UploadButton';
import { NameText } from './NameText';
import { ProcessSelection } from './ProcessSelection';
import { SubmitButton } from './SubmitButton';
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
