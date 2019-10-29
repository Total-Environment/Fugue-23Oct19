import moment from 'moment';

export const initialState = {
  componentData: {
    masterData: {}
  },
  masterData: {},
  masterDataByName: {},
  componentCreateError: {
    message: ''
  },
  componentUpdateError: {
    message: ''
  },
  components: {},
  dependencyDefinitions: {},
  assetDefinitions: {},
  definitions: {material: {}, service: {}},
  rentalRateHistory: {},
  loading: {
    status: false,
    message: ''
  },
  rates: {},
  rateFilters: {
    material: [
      {columnKey: 'material_status', columnValue: 'Approved'},
      {columnKey: 'Location', columnValue: 'Bangalore'},
      {columnKey: 'AppliedOn', columnValue: moment().toISOString()}
    ],
    service: [
      {columnKey: 'service_status', columnValue: 'Approved'},
      {columnKey: 'Location', columnValue: 'Bangalore'},
      {columnKey: 'AppliedOn', columnValue: moment().toISOString()}
    ]
  },
  bulkRates: {material: undefined, service: undefined},
  ratesEditable: {
    material: false,
    service: false,
  },
  ratesError: {
    material: '',
    service: ''
  },
  dependencyDefinitionError: {
    material :'',
    service: '',
    sfg: '',
    package: ''
  },
  bulkRateError: {},
  sfgCosts:{},
  packageCosts: {},
};
