import { Component, signal } from '@angular/core';
import { UploadButton } from './UploadButton';
import { NameText } from './NameText';
import { ProcessSelection } from './ProcessSelection';

@Component({
  selector: 'app-root',
  imports: [UploadButton, NameText, ProcessSelection],
  templateUrl: './app.html',
  styleUrl: './app.scss',
})
export class App {
  protected readonly title = signal('hisp-frontend');
}
