import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NotFoundComponent } from './not-found/not-found.component';
import { LogTestComponent } from './log-component/log-test.component';

const routes: Routes = [
  { path: '', redirectTo: '/search', pathMatch: 'full' },
  { path: 'provider', redirectTo: '/provider', pathMatch: 'full' },
  { path: 'facility', redirectTo: '/facility', pathMatch: 'full' },
  { path: 'logtest',component:LogTestComponent},
  {
    path: '**', component: NotFoundComponent,
    data: {
      title: '404 - Not found',
      meta: [{ name: 'description', content: '404 - Error' }]
    }
  }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [RouterModule]
})
export class AppRoutingModule {}
