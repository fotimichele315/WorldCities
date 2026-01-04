import { Injectable } from '@angular/core';
import { FormGroup, AbstractControl } from '@angular/forms';

@Injectable()
export abstract class BaseFormComponent {

  form!: FormGroup;

  getErrors(
    control: AbstractControl,
    displayName: string,
    customMessages: { [key: string]: string } | null = null
  ): string[] {

    const errors: string[] = [];

    Object.keys(control.errors || {}).forEach(key => {
      switch (key) {
        case 'required':
          errors.push(`${displayName} ${customMessages?.[key] ?? 'is required'}`);
          break;
        case 'pattern':
          errors.push(`${displayName} ${customMessages?.[key] ?? 'contains invalid characters'}`);
          break;
        case 'isDupeField':
          errors.push(`${displayName} ${customMessages?.[key] ?? 'already exists: please choose another'}`);
          break;
        default:
          errors.push(`${displayName} ${customMessages?.[key] ?? 'is invalid'}`);
      }
    });

    return errors;
  }

  constructor() { }
}
