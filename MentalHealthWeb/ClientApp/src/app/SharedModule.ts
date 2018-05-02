import { NgModule } from '@angular/core';
import { FormGroup, FormControl } from "@angular/forms";
import { FormsModule, ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActiveInactivePipe } from './pipes/ActiveInactivePipe';
import { BooleanPipe } from './pipes/BooleanPipe';
import { FormatTermDatePipe } from './pipes/FormatTermDatePipe';

@NgModule({
  imports: [
    CommonModule,
    FormsModule
  ],
  exports: [
    ActiveInactivePipe,
    BooleanPipe,
    FormatTermDatePipe
  ],
  declarations: [
    ActiveInactivePipe,
    BooleanPipe,
    FormatTermDatePipe
]
})

export class SharedModule {


}
