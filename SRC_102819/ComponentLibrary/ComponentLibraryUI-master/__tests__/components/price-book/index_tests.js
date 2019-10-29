import React from 'react';
import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import {PriceBook} from "../../../src/components/price-book/index";
import {PriceBookFilters} from "../../../src/components/price-book/filters";
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import moment from "moment";

chai.use(sinonChai);

describe('Pricebook', () => {
  let props, coefficients, cprs, wrapper;
  beforeEach(() => {
    coefficients = {
      "cprCoefficient": {
        "columns": [
          {
            "key": "coefficient1",
            "name": "Coefficient 1",
            "value": {
              "type": "%",
              "value": "10.1"

            }
          },
          {
            "key": "coefficient2",
            "name": "Coefficient 2",
            "value": {
              "type": "%",
              "value": "10.1"
            }
          },
          {
            "key": "coefficient3",
            "name": "Coefficient 3",
            "value": {
              "type": "%",
              "value": "30.3"

            }
          },
          {
            "key": "coefficient4",
            "name": "Coefficient 4",
            "value": {
              "type": "%",
              "value": "40.4"

            }
          },
          {
            "key": "coefficient5",
            "name": "Coefficient 5",
            "value": {
              "type": "%",
              "value": "40"

            }
          },
          {
            "key": "coefficient6",
            "name": "Coefficient 6",
            "value": {
              "type": "%",
              "value": "60.6"

            }
          },
          {
            "key": "coefficient7",
            "name": "Coefficient 7",
            "value": {
              "type": "%",
              "value": "70.7"

            }
          },
          {
            "key": "coefficient8",
            "name": "Profit Margin",
            "value": {
              "type": "%",
              "value": "40"

            }
          }
        ]
      },
    };
    cprs = [
      {
        "componentType": "Material",
        "appliedFrom": "2017-08-18T10:00:00.000Z",
        "projectCode": null,
        "level1": "Primary",
        "level2": "Aluminium & Copper",
        "level3": "Copper",
        ...coefficients,
      },
      {
        "componentType": "Material",
        "appliedFrom": "2017-08-18T10:00:00.000Z",
        "projectCode": null,
        "level1": "Primary",
        ...coefficients,
      },
      {
        "componentType": "Material",
        "appliedFrom": "2017-08-18T10:00:00.000Z",
        "projectCode": null,
        "level1": "Primary",
        "level2": "Clay Materials",
        ...coefficients,
      },
      {
        "componentType": "Material",
        "appliedFrom": "2017-08-18T10:00:00.000Z",
        "projectCode": null,
        "level1": "Primary",
        "level2": "Clay Materials",
        "level3": "Soil",
        ...coefficients,
      },
      {
        "componentType": "Material",
        "appliedFrom": "2017-08-18T10:00:00.000Z",
        "projectCode": null,
        "level1": "Primary",
        "level2": "Clay Materials",
        "level3": "Soil",
        "code": "CLY000001",
        ...coefficients,
      },
      {
        "componentType": "Material",
        "appliedFrom": "2017-08-18T10:00:00.000Z",
        "projectCode": null,
        "level1": "Primary",
        "level2": "Clay Materials",
        "level3": "Soil",
        "code": "CLY000002",
        ...coefficients,
      },
    ];
    props = {
      dependencyDefinitions: {
        materialClassifications: {isFetching: false, values: {}},
        serviceClassifications: {isFetching: false, values: {}},
        sfgClassifications: {isFetching: false, values: {}},
        packageClassifications: {isFetching: false, values: {}},
      },
      dependencyDefinitionError: {material: {}, service: {}, sfg: {}, package: {}},
      fetchCprs: sinon.spy(),
      cprs: {isFetching: false, values: cprs},
      projects: {isFetching: false, values: []}
    };
    wrapper = shallow(<PriceBook {...props}/>);
  });

  it('should call onFetchCprs when filters change', () => {
    const filterComponent = wrapper.find('PriceBookFilters');
    const date = moment('2017-06-29T10:05:45.658Z');
    filterComponent.simulate('change', {componentType: 'Service', level1: 'Primary', appliedOn: date});
    expect(props.fetchCprs).to.have.been.calledWith({
      componentType: 'Service',
      appliedOn: '2017-06-29T10:05:45.658Z',
      level1: 'Primary',
      projectCode: ''
    });
  });
  describe('render', () => {
    it('should render CPRTable with nested cprs', () => {
      const expected = {
        'Aluminium & Copper': {
          self: {},
          children: {
            'Copper': {
              self: {
                "componentType": "Material",
                "appliedFrom": "2017-08-18T10:00:00.000Z",
                "projectCode": null,
                "level1": "Primary",
                "level2": "Aluminium & Copper",
                "level3": "Copper",
                ...coefficients,
              },
              children: {},
            }
          }
        },
        'Clay Materials': {
          self: {
            "componentType": "Material",
            "appliedFrom": "2017-08-18T10:00:00.000Z", "projectCode": null,
            "level1": "Primary",
            "level2": "Clay Materials",
            ...coefficients,
          },
          children: {
            'Soil': {
              self: {
                "componentType": "Material",
                "appliedFrom": "2017-08-18T10:00:00.000Z",
                "projectCode": null,
                "level1": "Primary",
                "level2": "Clay Materials",
                "level3": "Soil",
                ...coefficients,
              },
              children: {
                'CLY000001': {
                  self: {
                    "componentType": "Material",
                    "appliedFrom": "2017-08-18T10:00:00.000Z",
                    "projectCode": null,
                    "level1": "Primary",
                    "level2": "Clay Materials",
                    "level3": "Soil",
                    "code": "CLY000001",
                    ...coefficients,
                  },
                  children: {},
                },
                'CLY000002': {
                  self: {
                    "componentType": "Material",
                    "appliedFrom": "2017-08-18T10:00:00.000Z",
                    "projectCode": null,
                    "level1": "Primary",
                    "level2": "Clay Materials",
                    "level3": "Soil",
                    "code": "CLY000002",
                    ...coefficients,
                  },
                  children: {},
                }
              }
            }
          }
        },
      };
      const filterComponent = wrapper.find('PriceBookFilters');
      filterComponent.simulate('change', {componentType: 'Material', level1: 'Primary'});
      expect(wrapper.find('CPRTable')).to.have.length(1);
      expect(wrapper.find('CPRTable')).to.have.prop('cprs').eql(expected);
    });

    it('should not render CPR table and render a message if component type is not selected', () => {
      const filterComponent = wrapper.find('PriceBookFilters');
      filterComponent.simulate('change', {componentType: ''});
      expect(wrapper.find('CPRTable')).to.have.length(0);
    });
    it('should not render CPR table and component type is material', () => {
      const filterComponent = wrapper.find('PriceBookFilters');
      filterComponent.simulate('change', {componentType: 'Material', level1: ''});
      expect(wrapper.find('CPRTable')).to.have.length(0);
    });
    it('should render CPR with filtered values based on classification filters', () => {
      const expected = {
        'Clay Materials': {
          self: {
            "componentType": "Material",
            "appliedFrom": "2017-08-18T10:00:00.000Z", "projectCode": null,
            "level1": "Primary",
            "level2": "Clay Materials",
            ...coefficients,
          },
          children: {
            'Soil': {
              self: {
                "componentType": "Material",
                "appliedFrom": "2017-08-18T10:00:00.000Z",
                "projectCode": null,
                "level1": "Primary",
                "level2": "Clay Materials",
                "level3": "Soil",
                ...coefficients,
              },
              children: {
                'CLY000001': {
                  self: {
                    "componentType": "Material",
                    "appliedFrom": "2017-08-18T10:00:00.000Z",
                    "projectCode": null,
                    "level1": "Primary",
                    "level2": "Clay Materials",
                    "level3": "Soil",
                    "code": "CLY000001",
                    ...coefficients,
                  },
                  children: {},
                },
                'CLY000002': {
                  self: {
                    "componentType": "Material",
                    "appliedFrom": "2017-08-18T10:00:00.000Z",
                    "projectCode": null,
                    "level1": "Primary",
                    "level2": "Clay Materials",
                    "level3": "Soil",
                    "code": "CLY000002",
                    ...coefficients,
                  },
                  children: {},
                }
              }
            }
          }
        },
      };
      const filterComponent = wrapper.find('PriceBookFilters');
      filterComponent.simulate('change', {
        componentType: 'Material',
        level1: 'Primary',
        level2: 'Clay Materials',
        level3: 'Soil'
      });
      expect(wrapper.find('CPRTable')).to.have.length(1);
      expect(wrapper.find('CPRTable')).to.have.prop('cprs').eql(expected);
    });
    it('should render CPR if there are no filtered values', () => {
      cprs = [
        {
          "componentType": "Service",
          "appliedFrom": "2017-08-18T10:00:00.000Z",
          "projectCode": null,
          "level1": "Flooring",
          "level2": "Dado",
          ...coefficients,
        },
        {
          "componentType": "Service",
          "appliedFrom": "2017-08-18T10:00:00.000Z",
          "projectCode": null,
          "level1": "Flooring",
          ...coefficients,
        },
      ];
      props.cprs.values = cprs;
      const expected = {
        'Flooring': {
          self: {
            "componentType": "Service",
            "appliedFrom": "2017-08-18T10:00:00.000Z",
            "projectCode": null,
            "level1": "Flooring",
            ...coefficients,
          },
          children: {
            'Dado': {
              self: {
                "componentType": "Service",
                "appliedFrom": "2017-08-18T10:00:00.000Z",
                "projectCode": null,
                "level1": "Flooring",
                "level2": "Dado",
                ...coefficients,
              },
              children: {}
            }
          }
        },
      };
      wrapper = shallow(<PriceBook {...props}/>);
      const filterComponent = wrapper.find('PriceBookFilters');
      filterComponent.simulate('change', {
        componentType: 'Service',
        level1: 'Flooring',
      });
      expect(wrapper.find('CPRTable')).to.have.length(1);
      expect(wrapper.find('CPRTable')).to.have.prop('cprs').eql(expected);
    });
    it('should render CPR if there are no filtered values', () => {
      cprs = [
        {
          "componentType": "Service",
          "appliedFrom": "2017-08-18T10:00:00.000Z",
          "projectCode": null,
          "level1": "Flooring",
          "level2": "Dado",
          ...coefficients,
        },
        {
          "componentType": "Service",
          "appliedFrom": "2017-08-18T10:00:00.000Z",
          "projectCode": null,
          "level1": "Flooring",
          ...coefficients,
        },
      ];
      props.cprs.values = cprs;
      const expected = {
        'Flooring': {
          self: {
            "componentType": "Service",
            "appliedFrom": "2017-08-18T10:00:00.000Z",
            "projectCode": null,
            "level1": "Flooring",
            ...coefficients,
          },
          children: {
            'Dado': {
              self: {
                "componentType": "Service",
                "appliedFrom": "2017-08-18T10:00:00.000Z",
                "projectCode": null,
                "level1": "Flooring",
                "level2": "Dado",
                ...coefficients,
              },
              children: {}
            }
          }
        },
      };
      wrapper = shallow(<PriceBook {...props}/>);
      const filterComponent = wrapper.find('PriceBookFilters');
      filterComponent.simulate('change', {
        componentType: 'Service',
        level1: 'Flooring',
      });
      expect(wrapper.find('CPRTable')).to.have.length(1);
      expect(wrapper.find('CPRTable')).to.have.prop('cprs').eql(expected);
    });
  });
});
