import { Component, effect, ElementRef, signal, ViewChild } from '@angular/core';
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
  needSigma = signal<boolean>(false);

  @ViewChild('sigmaInput') sigmaInput!: ElementRef<HTMLInputElement>;
  @ViewChild('radiusInput') radiusInput!: ElementRef<HTMLInputElement>;

  constructor(public svc: SelectedOpService) {
    effect(() => {
      const process: string = this.svc.userSelectedProcess();
      if (process === 'blur') {
        this.needRadius.set(true);
        this.needSigma.set(false);
      } else if (process === 'gblur') {
        this.needRadius.set(true);
        this.needSigma.set(true);
      } else if (process === 'gray') {
        this.needRadius.set(false);
        this.needSigma.set(false);
      } else {
        this.needRadius.set(false);
        this.needSigma.set(false);
      }
    });
  }

  updateSelectedOp(event: Event) {
    const select = event.target as HTMLSelectElement;
    console.log('Selected value:', select.value);
    this.svc.userSelectedProcess.set(select.value);
  }

  updateKernel(event: Event) {
    const kernelInput = event.target as HTMLInputElement;
    this.svc.kernelVal.set(kernelInput.value);
  }

  updateSigma(event: Event) {
    const sigmaInput = event.target as HTMLInputElement;
    this.svc.sigmaVal.set(sigmaInput.value);
  }
}
