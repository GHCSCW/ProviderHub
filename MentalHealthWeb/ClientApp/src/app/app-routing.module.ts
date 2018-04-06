import { NgModule }             from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { SearchComponent } from "./search/search.component";
import { NotFoundComponent } from './not-found/not-found.component';
//import { AddressComponent } from './address/address.component';

const routes: Routes = [
  { path: '', redirectTo: '/search', pathMatch: 'full' },
  { path: 'provider', redirectTo: '/provider', pathMatch: 'full' },
  { path: 'facility', redirectTo: '/facility', pathMatch: 'full' },
  {
    path: '**', component: NotFoundComponent,
    data: {
      title: '404 - Not found',
      meta: [{ name: 'description', content: '404 - Error' }],
      links: [
        { rel: 'canonical', href: 'http://blogs.example.com/bootstrap/something' },
        { rel: 'alternate', hreflang: 'es', href: 'http://es.example.com/bootstrap-demo' }
      ]
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
