import { ComponentFixture, TestBed } from '@angular/core/testing';
import { EditableText } from './editable-text';
import { NameText } from './name-text';

describe('EditableText', () => {
  let component: EditableText;
  let fixture: ComponentFixture<EditableText>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EditableText],
    }).compileComponents();

    fixture = TestBed.createComponent(EditableText);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});

describe('NameText', () => {
  let component: NameText;
  let fixture: ComponentFixture<NameText>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [NameText],
    }).compileComponents();

    fixture = TestBed.createComponent(NameText);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
