import { Component } from '@angular/core';
import { SelectedOpService } from './filter-selection.service';

@Component({
  selector: 'process-selection',
  templateUrl: 'process-selection.html',
})
export class ProcessSelection {
  constructor(public svc: SelectedOpService) {}

  updateSelectedOp(value: string) {
    this.svc.userSelectedProcess.set(value);
  }
}
