import React from 'react';
import ReactDOM, { render } from 'react-dom';
import { Provider } from 'react-redux';
import { reducer } from './reducers/reducers';
import { addDocumentReducer } from './reducers/addDocumentReducer';
import { applyMiddleware, createStore, combineReducers, compose } from 'redux';
import { ChecklistConnector } from './checklist_connector';
import { MaterialConnector } from './material_connector';
import { SfgConnector } from './sfg_connector';
import { CreateComponentConnector } from './create_component_connector';
import { Router, Route, browserHistory, IndexRoute, Redirect } from 'react-router';
import thunk from 'redux-thunk';
import { App } from './app';
import './index.css';
import { ServiceConnector } from './service_connector'
import { MaterialHomeConnector } from './material_home_connector';
import { ServiceHomeConnector } from './service_home_connector';
import { RateHistoryConnector } from './rate_History_connector';
import { ComponentSearchConnector } from './component_search_connector.js';
import { EditComponentConnector } from './edit_component_connector';
import { ExchangeRateConnector } from './exchange_rate_connector'
import { ExchangeRateHistoryConnector } from './exchange_rate_history_connector'
import { reducer as toastrReducer } from 'react-redux-toastr'
import axios from 'axios';
import ReduxToastr from 'react-redux-toastr';
import { RentalRateHistoryConnector } from './rental_rate_history_connector';
import { RatesHomeConnector } from './rates_home_connector';
import FontFaceObserver from 'fontfaceobserver';
import '../images/favicon.ico';
import { BrandConnector } from "./brand_connector";
import { ExtraInformationContent } from "./components/extra-information-content/index";
import '!style-loader!css-loader!./css-common/vendor/collapse.css';
import '!style-loader!css-loader!react-redux-toastr/lib/css/react-redux-toastr.css';
import '!style-loader!css-loader!react-datepicker/dist/react-datepicker.css';
import '!style-loader!css-loader!./css-common/vendor/pagination.css';
import '!style-loader!css-loader!react-image-gallery/styles/css/image-gallery.css';
import '!style-loader!css-loader!react-dd-menu/dist/react-dd-menu.min.css';
import '!style-loader!css-loader!./components/image-gallery/gallery.css';
import { EditRateConnector } from "./edit_rate_connector";
import { CompositeDetailsConnector } from './components/composite-details/connector';
import { SfgHome } from "./components/sfg-home/index";
import {apiUrl, logException} from "./helpers";
import { PackageConnector } from './package_connector';
import {PackageHomeConnector} from './package_home_connector';
import {PriceBookConnector} from "./components/price-book/connector";
import {PriceBookCreateConnector} from "./components/pricebook-create/connector";
import {updateSasToken} from "./auth";
import {logInfo} from "./sumologic-logger";

const acquireTokenPromise = (adal, clientId) => {
  return new Promise((resolve, reject) => {
    adal.acquireToken(clientId, (err, token) => {
      if (err !== null && err !== "") {
        reject(err);
      } else {
        resolve(token);
      }
    });
  });
};

axios.interceptors.request.use(async (config) => {
  try {
    const resource = adalInstance.getResourceForEndpoint(config.url);
    const token = await acquireTokenPromise(adalInstance, resource);
    config.headers.common['Authorization'] = 'Bearer ' + token;
    return config;
  } catch (error) {
    logException('ERROR DUE TO EXPIRY', error);
    adalInstance.login();
  }
});

const reducers = {
  reducer,
  addDocumentReducer,
  toastr: toastrReducer
};

const newReducer = combineReducers(reducers);
const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;
const store = createStore(newReducer, composeEnhancers(
  applyMiddleware(thunk),
));

const app = document.getElementById('main');

function start() {
  render(
    <Provider store={store}>
      <div>
        <Router history={browserHistory}>
          <Redirect from="/" to="/materials" />
          <Redirect from="/rates" to="/rates/materials" />
          <Route component={ExtraInformationContent}>
            {/*<Route path="/materials/:materialCode/brands/:brandCode" component={BrandConnector}/>*/}
          </Route>
          <Route path="/" component={App}>
            <Route path="materials">
              <IndexRoute component={MaterialHomeConnector} />
              <Route path="new" component={() => <CreateComponentConnector componentType="material" />} />
              <Route path="search/:group/:keyword" componentType="material" component={ComponentSearchConnector} />
              <Route path=":materialCode" component={MaterialConnector} />
              <Route path=":componentCode/edit" componentType="material" component={EditComponentConnector} />
              <Route path=":componentCode/rate-history" componentType="material" component={RateHistoryConnector} />
              <Route path=":materialCode/rental-rate-history" component={RentalRateHistoryConnector} />
              <Route path=":materialCode/brands/:brandCode" component={BrandConnector} />
              <Route path=":materialCode/brands/:brandCode/:action" component={BrandConnector} />
            </Route>
            <Route path="services">
              <IndexRoute component={ServiceHomeConnector} />
              <Route path="new" component={() => <CreateComponentConnector componentType="service" />} />
              <Route path="search/:group/:keyword" componentType="service" component={ComponentSearchConnector} />
              <Route path="rates">
                <IndexRoute componentType="service" component={RatesHomeConnector} />
              </Route>
              <Route path=":serviceCode" component={ServiceConnector} />
              <Route path=":componentCode/edit" componentType="service" component={EditComponentConnector} />
              <Route path=":componentCode/rate-history" componentType="service" component={RateHistoryConnector} />
            </Route>
            <Route path="sfgs">
              <IndexRoute component={SfgHome} />
              <Route path="new" componentType="sfg" mode="create" component={CompositeDetailsConnector} />
              <Route path=":sfgCode" componentType="sfg" component={SfgConnector} />
              <Route path=":sfgCode/edit" componentType="sfg" mode="edit" component={CompositeDetailsConnector} />
              <Route path="search/:keyword" componentType="sfg" component={ComponentSearchConnector} />
            </Route>
            <Route path="packages">
              <IndexRoute component={PackageHomeConnector} />
              <Route path="new" componentType="package" mode="create" component={CompositeDetailsConnector} />
              <Route path=":packageCode" component={PackageConnector} />
              <Route path=":packageCode/edit" componentType="package" mode="edit" component={CompositeDetailsConnector} />
              <Route path="search/:keyword" componentType="package" component={ComponentSearchConnector} />
            </Route>
            <Route path="exchange-rates">
              <IndexRoute component={ExchangeRateConnector} />
            </Route>
            <Route path="exchange-rates-history">
              <IndexRoute component={ExchangeRateHistoryConnector} />
            </Route>
            <Route path="rates">
              <Route path="materials" componentType="material" component={RatesHomeConnector} />
              <Route path="services" componentType="service" component={RatesHomeConnector} />
              <Route path="materials/edit" componentType="material" component={EditRateConnector} />
            </Route>
            <Route path="pricebook">
              <IndexRoute component={PriceBookConnector} />
              <Route path="new"  component={PriceBookCreateConnector} />
            </Route>
          </Route>
          <Route path="/check-lists/:checklistId" component={ChecklistConnector} />
        </Router>
        <ReduxToastr
          timeOut={0}
          newestOnTop={false}
          preventDuplicates={true}
          position="top-right"
          transitionIn="fadeIn"
          transitionOut="fadeOut" />
      </div>
    </Provider>,
    app);
}

async function fetchUserInfo() {
  const resource = window.adalInstance.getResourceForEndpoint(API_URL);
  const token = await acquireTokenPromise(window.adalInstance, resource);
  try {
    const response = await axios.get(
      apiUrl('user'),
      { headers: { Authorization: `Bearer ${ token }` } },
    );
    window.userPermissions = (response.data && response.data.permissions) || [];
    return response;
  } catch (error) {
    console.log(error);
  }
}

async function run() {
  await fetchUserInfo();
  await updateSasToken(60);
  logInfo('User logged in as \n' + window.userInformation.userName + '\n' + window.userInformation.email);
  start();
}

const weights = [300, 400, 700];
Promise.all(weights.map(weight => (new FontFaceObserver('Neue Haas Unica Pro', { weight: weight })).load())).then(() => {
  document.documentElement.className += " fonts-loaded";
});

run();
