import { Component, signal } from '@angular/core';
import { DisplayService } from '../../core/services/display.service';

@Component({
  selector: 'image-display',
  templateUrl: 'image-display.html',
})
export class ImageDisplay {
  constructor(public displaySvc: DisplayService) {}
  imgIdx = signal<number>(0);
}
