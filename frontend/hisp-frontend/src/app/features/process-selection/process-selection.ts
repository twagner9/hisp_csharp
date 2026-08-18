import { Component, signal } from '@angular/core';
import { SelectedOpService } from '../../core/services/prcoess-selection.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'process-selection',
  templateUrl: 'process-selection.html',
  standalone: true,
  imports: [FormsModule],
})
export class ProcessSelection {
  options = [
    { value: 'blur', label: 'Simple Blur' },
    { value: 'gblur', label: 'Gaussian Blur' },
    { value: 'grayscale', label: 'Grayscale' },
  ];

  // TODO: add user input for getting the kernel size; use boolean
  // based on the current selection (blur, grayscale, etc).
  needRadius = signal<boolean>(true);

  selectedOp = 'blur';
  constructor(public svc: SelectedOpService) {}

  updateSelectedOp(event: Event) {
    const select = event.target as HTMLSelectElement;
    console.log('Selected value:', select.value);
    this.svc.userSelectedProcess.set(select.value);
  }
}
