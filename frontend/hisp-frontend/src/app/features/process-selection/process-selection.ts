import { Component, computed, effect, ElementRef, signal, ViewChild } from '@angular/core';
import { SelectedOpService } from '../../core/services/prcoess-selection.service';
import { FormsModule } from '@angular/forms';

/**
 * OperationFlags specifies which operations are available, and
 */
interface OperationFlags {
  radius: boolean;
  sigma: boolean;
  delta: boolean;
  threshold: boolean;
}

const NO_FLAGS: OperationFlags = {
  radius: false,
  sigma: false,
  delta: false,
  threshold: false,
};

const OPERATION_FLAGS: Readonly<Record<string, OperationFlags>> = {
  blur: {
    radius: true,
    sigma: false,
    delta: false,
    threshold: false,
  },
  gblur: {
    radius: true,
    sigma: true,
    delta: false,
    threshold: false,
  },
  gray: NO_FLAGS,
  adj_brightness: {
    radius: false,
    sigma: false,
    delta: true,
    threshold: false,
  },
  simple_threshold: {
    radius: false,
    sigma: false,
    delta: false,
    threshold: true,
  },
};

@Component({
  selector: 'process-selection',
  templateUrl: 'process-selection.html',
  styleUrl: 'process-selection.css',
  standalone: true,
  imports: [FormsModule],
})
export class ProcessSelection {
  options = [
    { value: 'blur', label: 'Simple Blur' },
    { value: 'gblur', label: 'Gaussian Blur' },
    { value: 'grayscale', label: 'Grayscale' },
    { value: 'adj_brightness', label: 'Adjust Brightness' },
    { value: 'simple_threshold', label: 'Simple Global Threshold' },
  ];

  @ViewChild('sigmaInput') sigmaInput!: ElementRef<HTMLInputElement>;
  @ViewChild('radiusInput') radiusInput!: ElementRef<HTMLInputElement>;
  @ViewChild('deltaInput') deltaInput!: ElementRef<HTMLInputElement>;
  @ViewChild('thresholdInput') thresholdInput!: ElementRef<HTMLInputElement>;

  constructor(public svc: SelectedOpService) {}
  readonly flags = computed(() => OPERATION_FLAGS[this.svc.userSelectedProcess()] ?? NO_FLAGS);

  readonly needRadius = computed(() => this.flags().radius);
  readonly needSigma = computed(() => this.flags().sigma);
  readonly needDelta = computed(() => this.flags().delta);
  readonly needThreshold = computed(() => this.flags().threshold);

  updateSelectedOp(event: Event) {
    const select = event.target as HTMLSelectElement;
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

  updateDelta(event: Event) {
    const deltaInput = event.target as HTMLInputElement;
    this.svc.deltaVal.set(deltaInput.value);
  }

  updateThreshold(event: Event) {
    const thresholdInput = event.target as HTMLInputElement;
    this.svc.thresholdVal.set(thresholdInput.value);
  }
}
