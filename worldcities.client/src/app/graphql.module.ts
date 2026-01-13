import { provideApollo } from 'apollo-angular';
import { HttpLink } from 'apollo-angular/http';
import { inject, NgModule } from '@angular/core';
import { ApolloClient, InMemoryCache } from '@apollo/client/core';
import type { ApolloClientOptions } from '@apollo/client/core';
import { environment } from '../environments/environment';

 
const uri = environment.baseUrl + 'api/graphql';


export function createApollo() {
  const httpLink = inject(HttpLink);
  const uri = environment.baseUrl + 'api/graphql';

  return {
    link: httpLink.create({ uri }),
    cache: new InMemoryCache(),
  };
}
@NgModule({
  providers: [provideApollo(createApollo)],
})
export class GraphQLModule { }
