import {Component, signal} from "@angular/core"

@Component ({
    selector: "upload-button",
    template: `<button (click)="uploadButtonClick()">Upload Image</button>`
})

export class UploadButton {
    uploadButtonClick() {
        console.log("Hello ")
    }
}