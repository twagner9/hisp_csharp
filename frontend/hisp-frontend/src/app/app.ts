import { Component, signal } from '@angular/core';
import { EditableText } from './EditableText';

@Component({
  selector: 'app-root',
  imports: [EditableText],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  protected readonly title = signal('hisp-frontend');
}
