import {Component, signal} from "@angular/core"
import { UserNameService } from "./user-name.service";
import { EditableText } from "./EditableText";

@Component ({
    selector: "name-text",
	imports: [EditableText],
    template: `<h1>Hello <editable-text [startingText]="'Anonymous'"></editable-text>, ready to process images?</h1>`,
})

export class NameText {
	constructor(public svc: UserNameService) {}

}