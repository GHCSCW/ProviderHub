import { NgModule } from '@angular/core';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActiveInactivePipe } from './pipes/active-inactive.pipe';
import { BooleanPipe } from './pipes/boolean.pipe';
import { FormatTermDatePipe } from './pipes/format-term-date.pipe';
import { ENumAsStringPipe } from './pipes/enum.pipe';

@NgModule({
  imports: [
    CommonModule,
    FormsModule
  ],
  exports: [
    ActiveInactivePipe,
    BooleanPipe,
    FormatTermDatePipe,
    ENumAsStringPipe
  ],
  declarations: [
    ActiveInactivePipe,
    BooleanPipe,
    FormatTermDatePipe,
    ENumAsStringPipe
]
})

export class SharedModule {


}
