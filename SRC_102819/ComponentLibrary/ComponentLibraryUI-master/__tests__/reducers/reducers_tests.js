import {reducer} from '../../src/reducers/reducers';
import chai, {expect} from 'chai';

describe('reducer', () => {

  describe('checklist', () => {
    it('should return state contains checklist when action type is CHECKLIST_FETCH_SUCCEEDED', () => {
      const initialState = {};
      const action = {type: 'CHECKLIST_FETCH_SUCCEEDED', checklist: 'I am checklist'};
      const actual = reducer(initialState, action);
      expect(actual.checklist).to.equal(action.currentChecklistDetails);
    });

    it('should return previous state when action type is CHECKLIST_FETCH_FAILED', () => {
      const intialState = {};
      const action = {type: 'CHECKLIST_FETCH_FAILED', error: 'error'};
      const actual = reducer(intialState, action);
      const expected = {currentChecklistDetails: null, error: 'error'};
      expect(actual).to.deep.equal(expected);
    });

    it('should return previous state when action type doesn\'t exist ', () => {
      const initialState = {};
      const action = {type: 'INVALID_TYPE'};
      const actual = reducer(initialState, action);
      expect(actual).to.equal(initialState);
    });
  });

  describe('serviceData', () => {
    it('should return a state with material details when action type is SERVICE_FETCH_SUCCEEDED', () => {
      const intialState = {};
      const action = {
        type: 'SERVICE_FETCH_SUCCEEDED',
        details: {serviceDetails: {id: 'SR0001'}, serviceName: 'Service'}
      };
      const actualState = reducer(intialState, action);
      const expectedState = {
        components: {SR0001: {serviceDetails: {id: 'SR0001'}, serviceName: 'Service'}}, error: ''
      };
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with error message and details as null', () => {
      const intialState = {};
      const action = {type: 'SERVICE_FETCH_FAILED', serviceCode: 'SR0001', error: 'error has occured'};
      const actualState = reducer(intialState, action);
      const expectedState = {components: {'SR0001': null}, error: 'error has occured'};
      expect(actualState).to.deep.equal(expectedState);
    });
  });


  describe('materialData', () => {
    it('should return a state contains material details when action type is MATERIAL_FETCH_SUCCEEDED', () => {
      const intialState = {};
      const action = {
        type: 'MATERIAL_FETCH_SUCCEEDED',
        details: {materialName: 'MT0001', id: 'MT0001'}
      };
      const actualState = reducer(intialState, action);
      const expectedState = {components: {'MT0001': {materialName: 'MT0001', id: 'MT0001'}}, newMaterialError: ''};
      expect(actualState).to.deep.equal(expectedState);
    });

    it('should return a state with error message and details as null', () => {
      const intialState = {};
      const action = {type: 'MATERIAL_FETCH_FAILED', materialCode: 'MT0001', detailserror: 'error has occured'};
      const actualState = reducer(intialState, action);
      const expectedState = {components: {'MT0001': null}, newMaterialError: 'error has occured'};
      expect(actualState).to.deep.equal(expectedState);
    });

    it('should destroy material data on MATERIAL_DESTROYED action', () => {
      const initialState = {
        currentMaterialDetails: 'some Data',
        currentMaterialRates: 'some data',
        error: 'some error'
      };
      const action = {type: 'MATERIAL_DESTROYED'};
      const finalState = reducer(initialState, action);
      const expectedState = {
        currentMaterialDetails: null,
        currentMaterialRates: null,
        error: 'some error',
        rateserror: ""
      };
      expect(finalState).to.deep.equal(expectedState);
    })
  });


  describe('RateHistory', () => {
    it('should return a state contains rate history when action type is RATE_HISTORY_FETCH_SUCCEEDED', () => {
      const intialState = {};
      const action = {
        type: 'RATE_HISTORY_FETCH_SUCCEEDED',
        details: [{typeOfPurchase: "DOMESTIC INTER-STATE", location: "Hyderabad"}],
        error: ''
      };
      const actualState = reducer(intialState, action);
      const expectedState = {rateHistory: [{typeOfPurchase: "DOMESTIC INTER-STATE", location: "Hyderabad"}], rateHistoryError: ''};
      expect(actualState).to.deep.equal(expectedState);
    });

    it('should return a state with error message when action type is RATE_HISTORY_FETCH_FAILED', () => {
      const intialState = {};
      const action = {type: 'RATE_HISTORY_FETCH_FAILED', error: 'rate history is not found'};
      const actualState = reducer(intialState, action);
      const expectedState = {rateHistory: null, rateHistoryError: 'rate history is not found'};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with rateAdding true when called with ADD_RATE_REQUESTED', () => {
      const action = {type: 'ADD_RATE_REQUESTED'};
      const actualState = reducer({}, action);
      const expectedState = {rateAdding: true};
      expect(actualState).to.deep.equals(expectedState);
    });
    it('should return a state with rateAdding false when called with ADD_RATE_SUCCEEDED', () => {
      const action = {type: 'ADD_RATE_SUCCEEDED'};
      const actualState = reducer({}, action);
      const expectedState = {rateAdding: false};
      expect(actualState).to.deep.equals(expectedState);
    });
    it('should return a state with rateAdding false and addRateError error message when called with ADD_RATE_FAILED', () => {
      const action = {type: 'ADD_RATE_FAILED', error: 'error'};
      const actualState = reducer({}, action);
      const expectedState = {rateAdding: false, addRateError: 'error'};
      expect(actualState).to.deep.equals(expectedState);
    });
    it('should return a state with addRateError as null when called with ADD_RATE_ERROR_CLOSED', () => {
      const action = {type: 'ADD_RATE_ERROR_CLOSED'};
      const actualState = reducer({}, action);
      const expectedState = {addRateError: null};
      expect(actualState).to.deep.equals(expectedState);
    });
    it('should return a state with rateHistory when called with rateHistory', () => {
      const action = {type: 'DESTROY_RATE_HISTORY'};
      const actualState = reducer({}, action);
      const expectedState = {rateHistory: null, rateHistoryError: null};
      expect(actualState).to.deep.equals(expectedState);
    });
  });

  describe('materialRates', () => {
    it('should return a state contains material rates when action type is MATERIAL_RATES_FETCH_SUCCEEDED', () => {
      const intialState = {};
      const action = {
        type: 'MATERIAL_RATES_FETCH_SUCCEEDED',
        rates: [{typeOfPurchase: "DOMESTIC INTER-STATE", location: "Hyderabad"}]
      };
      const actualState = reducer(intialState, action);
      const expectedState = {
        currentMaterialRates: [
          {
            typeOfPurchase: "DOMESTIC INTER-STATE",
            location: "Hyderabad"
          }
        ],
        "rateserror": ""
      };
      expect(actualState).to.deep.equal(expectedState);
    });

    it('should return a state with error message and details as null', () => {
      const intialState = {};
      const action = {type: 'MATERIAL_RATES_FETCH_FAILED', rateserror: 'error has been occured'};
      const actualState = reducer(intialState, action);
      const expectedState = {currentMaterialRates: null, rateserror: 'error has been occured'};
      expect(actualState).to.deep.equal(expectedState);
    });
  });

  describe('ServiceRates', () => {
    it('should return a state contains service rates when action type is SERVICE_RATES_FETCH_SUCCEEDED', () => {
      const initialState = {};
      const action = {
        type: 'SERVICE_RATES_FETCH_SUCCEEDED',
        rates: [{
          "typeOfPurchase": "IMPORT",
          "location": "Hyderabad",
          "id": "IRN000013"
        }]
      };

      const actualState = reducer(initialState, action);
      const expectedState = {
        serviceRates: [{
          "typeOfPurchase": "IMPORT",
          "location": "Hyderabad",
          "id": "IRN000013"
        }], rateserror: ''
      };

      expect(actualState).to.deep.equal(expectedState);
    });

    it('should return a state with error message and details as null', () => {
      const intialState = {};
      const action = {type: 'SERVICE_RATES_FETCH_FAILED', error: 'error has been occured'};
      const actualState = reducer(intialState, action);
      const expectedState = {serviceRates: null, rateserror: 'error has been occured'};
      expect(actualState).to.deep.equal(expectedState);
    });
  });

  describe('RentalRates', () => {
    it('should return a state contains rental rates when action type is RENTAL_RATES_FETCH_SUCCEEDED', () => {
      const initialState = {};
      const action = {
        type: 'RENTAL_RATES_FETCH_SUCCEEDED',
        rentalRates: [{
          "unitOfMeasure": "Daily",
          "id": "IRN000013"
        }]
      };

      const actualState = reducer(initialState, action);
      const expectedState = {
        currentRentalRates: [{
          "unitOfMeasure": "Daily",
          "id": "IRN000013"
        }], rentalRatesError: null
      };
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with error message and details as null', () => {
      const initialState = {};
      const action = {type: 'RENTAL_RATES_FETCH_FAILED', rentalRatesError: 'error has been occured'};
      const actualState = reducer(initialState, action);
      const expectedState = {currentRentalRates: null, rentalRatesError: 'error has been occured'};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with unitOfMeasureForRentalRate from the action', () => {
      const action = {type: 'MASTER_DATA_FOR_RENTAL_RATE_FETCH_SUCCEEDED', unitOfMeasureForRentalRate: "some data"};
      const actualState = reducer({}, action);
      const expectedState = {unitOfMeasureForRentalRate: "some data"};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with unitOfMeasureForRentalRateError from the action', () => {
      const action = {type: 'MASTER_DATA_FOR_RENTAL_RATE_FETCH_FAILED', unitOfMeasureForRentalRateError: 'some error'};
      const actualState = reducer({}, action);
      const expectedState = {unitOfMeasureForRentalRateError: 'some error'};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with addRentalRateError from the action', () => {
      const action = {type: 'ADD_RENTAL_RATE_ERROR_CLOSED'};
      const actualState = reducer({}, action);
      const expectedState = {addRentalRateError: null};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with rentalRateAdding with true from the action', () => {
      const action = {type: 'ADD_RENTAL_RATE_REQUESTED'};
      const actualState = reducer({}, action);
      const expectedState = {rentalRateAdding: true};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with rentalRateAdding with false from the action', () => {
      const action = {type: 'ADD_RENTAL_RATE_SUCCEEDED'};
      const actualState = reducer({}, action);
      const expectedState = {rentalRateAdding: false};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with rentalRateAdding as false with error msg from the action', () => {
      const action = {type: 'ADD_RENTAL_RATE_FAILED', error: 'some error'};
      const actualState = reducer({}, action);
      const expectedState = {addRentalRateError: 'some error', rentalRateAdding: false};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should set data and turn off is fetching when data fetched', () => {
      const action = {
        type: 'RENTAL_RATE_HISTORY_FETCH_SUCCEEDED',
        rentalRateHistory: {'code': 'somecode', 'data': 'some data'}
      };
      const actualState = reducer({
        rentalRateHistory: {
          'somecode': {
            isFetching: true,
            values: undefined,
            error: undefined
          }
        }
      }, action);
      const expectedState = {
        rentalRateHistory: {
          'somecode': {
            isFetching: false,
            values: 'some data',
            error: undefined
          }
        }
      };
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with error msg from the action', () => {
      const action = {type: 'RENTAL_RATE_HISTORY_FETCH_FAILED', materialCode: 'MCH001', error: 'some error'};
      const actualState = reducer({rentalRateHistory: {'MCH001': {isFetching: true, values: undefined}}}, action);
      const expectedState = {
        rentalRateHistory: {
          'MCH001': {
            isFetching: false,
            values: undefined,
            error: 'some error'
          }
        }
      };
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with empty currentRentalRateHistory when destroyed', () => {
      const action = {type: 'RENTAL_RATE_HISTORY_DESTROY', materialCode: 'MCH001'};
      const actualState = reducer({rentalRateHistory: {'MCH001': {}}}, action);
      const expectedState = {rentalRateHistory: {}};
      expect(actualState).to.deep.equal(expectedState);
    });
    it('should return a state with isFetching true on RENTAL_RATE_HISTORY_FETCH_STARTED', () => {
      const action = {type: 'RENTAL_RATE_HISTORY_FETCH_STARTED', materialCode: 'MCH001'};
      const actualState = reducer({rentalRateHistory: {}}, action);
      expect(actualState).to.deep.equal({
        rentalRateHistory: {'MCH001': {isFetching: true, values: undefined, error: undefined}}
      });
    });
  });

  describe('component', () => {
    describe('COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED', () => {
      it('should return a state containing component definition', () => {
        const initialState = {};
        const action = {type: 'COMPONENT_MATERIAL_DEFINITION_FETCH_SUCCEEDED', definition: {name: 'Clay Material'}};
        const actualState = reducer(initialState, action);
        const expectedState = {definitions: {'Clay Material': {name: 'Clay Material'}}};
        expect(actualState).to.deep.equal(expectedState);
      });
    });

    describe('COMPONENT_BRAND_DEFINITION_FETCH_SUCCEEDED', () => {
      it('should return a state containing brand definition', () => {
        const initialState = {};
        const action = {
          type: 'COMPONENT_BRAND_DEFINITION_FETCH_SUCCEEDED',
          brandDefinition: {name: 'Generic Brand Definition'}
        };
        const actualState = reducer(initialState, action);
        const expectedState = {brandDefinitions: {'Generic Brand Definition': {name: 'Generic Brand Definition'}}};
        expect(actualState).to.deep.equal(expectedState);
      });
    });

    describe('COMPONENT_BRAND_DEFINITION_FETCH_FAILED', () => {
      it('should return error when state not containing brand definition', () => {
        const initialState = {};
        const action = {type: 'COMPONENT_BRAND_DEFINITION_FETCH_FAILED', error: 'error'};
        const actualState = reducer(initialState, action);
        const expectedState = {brandDefinitions: null, error: 'error'};
        expect(actualState).to.deep.equal(expectedState);
      });
    });


    describe('COMPONENT_DEFINITION_FETCH_FAILED', () => {
      it('should return state with generic definition as null', () => {
        const initialState = {genericDefinition: 1};
        const action = {type: 'COMPONENT_DEFINITION_FETCH_FAILED'};
        const actualState = reducer(initialState, action);
        const expected = {genericDefinition: null};
        expect(actualState).to.deep.equal(expected);
      });
    });


    describe('COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED', () => {
      it('should return a state containing component definition', () => {
        const initialState = {};
        const action = {type: 'COMPONENT_SERVICE_DEFINITION_FETCH_SUCCEEDED', definition: {name: 'Clay Service'}};
        const actualState = reducer(initialState, action);
        const expectedState = {definitions: {'Clay Service': {name: 'Clay Service'}}};
        expect(actualState).to.deep.equal(expectedState);
      });
    });

    describe('COMPONENT_MASTER_DATA_FETCH_SUCCEEDED', () => {
      let expectedState, action;
      beforeEach(() => {
        action = {masterData: {id: 'abc', values: [1, 2, 3]}, type: 'COMPONENT_MASTER_DATA_FETCH_SUCCEEDED'};
        expectedState = {masterData: {abc: {values: [1, 2, 3], status: 'fetched'}}};
      });
      it('should return state with masterData id set', () => {
        const initialState = {masterData: {}};
        expect(reducer(initialState, action)).to.deep.equal(expectedState);
      });

      it('should return state when masterData is not present', () => {
        const initialState = {};
        expect(reducer(initialState, action)).to.deep.equal(expectedState);
      });
    });

    describe('COMPONENT_MASTER_DATA_FETCH_FAILED', () => {
      it('should return state with ComponentMasterDataError', () => {
        const initialState = {naam: 'hatadiya'};
        const action = {type: 'COMPONENT_MASTER_DATA_FETCH_FAILED', error: 'error'};
        expect(reducer(initialState, action)).to.deep.equal(Object.assign({}, initialState, {ComponentMasterDataError: 'error'}));
      });
    });
    describe('ADD_MATERIAL_FAILED', () => {
      it('should return state with error', () => {
        const action = {type: 'ADD_MATERIAL_FAILED', error: 'error'};
        expect(reducer({}, action)).to.deep.equal({componentCreateError: 'error', componentAdding: false});
      });
    });
    describe('ADD_SERVICE_FAILED', () => {
      it('should return state with error', () => {
        const action = {type: 'ADD_SERVICE_FAILED', error: 'error'};
        expect(reducer({}, action)).to.deep.equal({componentCreateError: 'error', componentAdding: false});
      });
    });
    describe('ADD_COMPONENT_REQUESTED', () => {
      it('should return state with componentAdding', () => {
        const action = {type: 'ADD_COMPONENT_REQUESTED'};
        expect(reducer({}, action)).to.deep.equal({componentAdding: true});
      });
    });
    describe('ADD_COMPONENT_SUCCEEDED', () => {
      it('should return state with componentAdding', () => {
        const action = {type: 'ADD_COMPONENT_SUCCEEDED'};
        expect(reducer({}, action)).to.deep.equal({componentAdding: false});
      });
    });

    describe('brandData', () => {
      it('should return a state contains material details when action type is BRAND_FETCH_SUCCEEDED', () => {
        const intialState = {};
        const action = {
          type: 'BRAND_FETCH_SUCCEEDED',
          details: {materialName: 'MT0001', id: 'MT0001'}
        };
        const actualState = reducer(intialState, action);
        const expectedState = {components: {'MT0001': {materialName: 'MT0001', id: 'MT0001'}}, fetchBrandError: ''};
        expect(actualState).to.deep.equal(expectedState);
      });

      it('should return a state with error message and details as null when action type is BRAND_FETCH_FAILED', () => {
        const intialState = {};
        const action = {type: 'BRAND_FETCH_FAILED', materialCode: 'MT0001', detailserror: 'error has occured'};
        const actualState = reducer(intialState, action);
        const expectedState = {components: {'MT0001': null}, fetchBrandError: 'error has occured'};
        expect(actualState).to.deep.equal(expectedState);
      });

      it('should return a state with brandAdding as true when action type is ADD_BRAND_REQUESTED', () => {
        const intialState = {};
        const action = {
          type: 'ADD_BRAND_REQUESTED'
        };
        const actualState = reducer(intialState, action);
        const expectedState = {brandAdding: true};
        expect(actualState).to.deep.equal(expectedState);
      });

      it('should return a state with updated material details when action type is ADD_BRAND_SUCCEEDED', () => {
        const intialState = {};
        const action = {
          type: 'ADD_BRAND_SUCCEEDED',
          details: {materialName: 'MT0001', id: 'MT0001'}
        };
        const actualState = reducer(intialState, action);
        const expectedState = {
          components: {'MT0001': {materialName: 'MT0001', id: 'MT0001'}},
          newBrandError: '',
          brandAdding: false,
          addedBrand: true
        };
        expect(actualState).to.deep.equal(expectedState);
      });

      it('should return a state with with error when action type is ADD_BRAND_FAILED', () => {
        const intialState = {};
        const action = {
          type: 'ADD_BRAND_FAILED',
          error: "Some error occured"
        };
        const actualState = reducer(intialState, action);
        const expectedState = {
          newBrandError: "Some error occured",
          brandAdding: false
        };
        expect(actualState).to.deep.equal(expectedState);
      });
    });

    describe('brandDefinition', () => {
      it('should return a state contains brand definition when action type is BRAND_DEFINITION_FETCH_SUCCEEDED', () => {
        const intialState = {};
        const action = {
          type: 'BRAND_DEFINITION_FETCH_SUCCEEDED',
          brandDefinition: {"Name": "Generic Brand"}
        };
        const actualState = reducer(intialState, action);
        const expectedState = {brandDefinition: {"Name": "Generic Brand",}, brandDefinitionError: ''};
        expect(actualState).to.deep.equal(expectedState);
      });

      it('should return a state with error message and details as null when action type is BRAND_DEFINITION_FETCH_FAILED', () => {
        const intialState = {};
        const action = {type: 'BRAND_DEFINITION_FETCH_FAILED', error: 'error has occured'};
        const actualState = reducer(intialState, action);
        const expectedState = {brandDefinition: null, brandDefinitionError: 'error has occured'};
        expect(actualState).to.deep.equal(expectedState);
      });
    });


    describe('UpdateMaterial', () => {
      it('should return a state contains component adding false when action type is UPDATE_MATERIAL_SUCCEEDED', () => {
        const action = {type: 'UPDATE_MATERIAL_SUCCEEDED', id: 'IRN0001'};
        const initialState = {};
        const actualState = reducer(initialState, action);
        expect(actualState).to.deep.equal({components: {'IRN0001': null}, componentUpdating: false});
      });

      it('should return a state contains component adding false and error when action type is UPDATE_MATERIAL_FAILED', () => {
        const action = {type: 'UPDATE_MATERIAL_FAILED', error: "error has occurred"};
        const initialState = {};
        const actualState = reducer(initialState, action);
        expect(actualState).to.deep
          .equal({componentUpdating: false, componentUpdateError: "error has occurred"});
      });
    });

    describe('UpdateService', () => {
      it('should return a state contains component adding false when action type is UPDATE_SERVICE_SUCCEEDED', () => {
        const action = {type: 'UPDATE_SERVICE_SUCCEEDED', id: 'FDP0001'};
        const initialState = {};
        const actualState = reducer(initialState, action);
        expect(actualState).to.deep.equal({components: {'FDP0001': null}, componentUpdating: false});
      });

      it('should return a state contains component adding false and error when action type is UPDATE_SERVICE_FAILED', () => {
        const action = {type: 'UPDATE_SERVICE_FAILED', error: "error has occurred"};
        const initialState = {};
        const actualState = reducer(initialState, action);
        expect(actualState).to.deep
          .equal({componentUpdating: false, componentUpdateError: "error has occurred"});
      });
    });

    describe('DependencyDefinition', () => {
      it('should return a state contains dependencyDefinitionError when action type is DEPENDENCY_DEFINITION_FETCH_FAILED', () => {
        const action = {type: 'DEPENDENCY_DEFINITION_FETCH_FAILED', error: 'error', dependencyDefinitionId: 'sfgClassification', componentType: 'material'};
        const initialState = {dependencyDefinitions: {sfgClassification : {isFetching : true}},dependencyDefinitionError: {material:{}, service: {}} };
        const actualState = reducer(initialState, action);
        expect(actualState).to.deep.equal(Object.assign({}, initialState, {dependencyDefinitionError: {material: 'error', service: {}}}, {dependencyDefinitions: {sfgClassification : {isFetching : false}}}));
      });
    });

    describe('ResetUpdateError', () => {
      it('should return a new state that contains componentUpdateError with empty value', () => {
        const action = {type: 'RESET_UPDATE_ERROR'};
        const initialState = {};
        const actualState = reducer(initialState, action);
        expect(actualState).to.deep
          .equal({componentUpdateError: ""});
      });
    });

    describe('BULK_EDIT_RESPONSE', () => {
      it('should update Rates with response data', () => {
        const initialState = {
          rates: {
            material: [{
              materialName: 'Clay Material',
              materialRate: {
                "id": "FDP1027",
              }
            }]
          },
        };
        const state = reducer(initialState, {
          type: 'BULK_EDIT_RESPONSE',
          componentType: 'material',
          records: [{
            "message": "Applied on date cannot be set to past.",
            "recordData": [{
              "rate": 1,
              "id": "FDP1027",
            }],
            "status": "Error"
          }],
          status: 'Failed'
        });
        expect(state.rates.material).to.eql([{
          materialName: 'Clay Material',
          materialRate: {
            "rate": 1,
            "id": "FDP1027",
          }
        }])
      })
    });
  });
});
