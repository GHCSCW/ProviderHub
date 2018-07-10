import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { SearchComponent } from './search.component';
import { AdvancedSearchComponent } from './advanced.search.component';

const providerRoutes: Routes = [
  { path: '', component: SearchComponent },
  { path: 'search', component: SearchComponent },
  { path: 'advanced-search', component: AdvancedSearchComponent },
];

@NgModule({
  imports: [
    RouterModule.forChild(providerRoutes)
  ],
  exports: [
    RouterModule
  ]
})
export class SearchRoutingModule { }
