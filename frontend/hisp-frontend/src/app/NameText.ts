import {Component, signal} from "@angular/core"

@Component ({
    selector: "upload-button",
    template: `<h1>Hello {{userName()}}, ready to process images?</h1>`,
})

export class UploadButton {
    userName = signal("Anonymous");
}