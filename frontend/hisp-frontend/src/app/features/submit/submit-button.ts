import { Component, computed, effect } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { SelectedOpService } from '../../core/services/prcoess-selection.service';
import { DisplayService } from '../../core/services/display.service';

@Component({
  selector: 'submit-button',
  template: `<button (click)="submitProcessingJob()" [disabled]="!readyForSubmission()">
    Submit
  </button>`,
})
export class SubmitButton {
  readyForSubmission = computed(() => {
    const data = this.selectionSvc.image();
    return data !== null && data !== undefined;
  });

  constructor(
    public selectionSvc: SelectedOpService,
    public displaySvc: DisplayService,
    private http: HttpClient,
  ) {}
  submitProcessingJob() {
    // Pass the userSelectedProcess to the backend along with the image data
    const formData: FormData = new FormData();
    const image = this.selectionSvc.image();
    if (image !== null) {
      formData.append('image', image);
    } else {
      console.error('Passed null image when submitting job.');
      return;
    }

    console.log(`submitProcessingJob() has ${this.selectionSvc.userSelectedProcess()}`);
    if (
      this.selectionSvc.userSelectedProcess() === 'blur' ||
      this.selectionSvc.userSelectedProcess() === 'gblur'
    ) {
      formData?.append('kernelRadius', this.selectionSvc.kernelVal().toString());

      if (this.selectionSvc.userSelectedProcess() === 'gblur') {
        formData?.append('sigma', this.selectionSvc.sigmaVal().toString());
      }
    }

    console.log([...formData!.entries()]);

    this.http
      .post(
        `http://localhost:5192/api/Image/process/${this.selectionSvc.userSelectedProcess()}`,
        formData,
        {
          responseType: 'blob',
        },
      )
      .subscribe({
        next: (blob) => {
          console.log('Backend received', blob);
          console.log('MIME type:', blob.type);
          console.log('Size: ', blob.size, 'bytes');
          const imageUrl = URL.createObjectURL(blob);
          this.displaySvc.mostRecentlyProcessedImg.set(imageUrl);
          this.displaySvc.processedImages.update((images) => [...images, imageUrl]);
          console.log('Num processed imgs: ', this.displaySvc.processedImages().length);
        },
        error: (error) => {
          if (error.error instanceof Blob) {
            error.error.text().then((message: string) => {
              console.error('Backend error:', message);
            });
          }
          console.error('Upload failed when submitting processing job:', error);
        },
        complete: () => {
          console.log('Successfully transmitted.');
        },
      });
  }
}
