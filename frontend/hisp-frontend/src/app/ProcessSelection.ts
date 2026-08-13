import { Component } from '@angular/core';
import { SelectedOpService } from './filter-selection.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'process-selection',
  templateUrl: 'process-selection.html',
  standalone: true,
  imports: [FormsModule],
})
export class ProcessSelection {
  selectedOp = 'blur';
  constructor(public svc: SelectedOpService) {}

  updateSelectedOp(value: string) {
    this.svc.userSelectedProcess.set(value);
  }
}
