import { Component, signal } from '@angular/core';

@Component({
  selector: 'submit-button',
  templateUrl: 'submit-button.html',
})
export class SubmitButton {
  onSubmitClick() {
    // Submit job to backend
    // TODO: inject the filter type so it's visible
  }
}
