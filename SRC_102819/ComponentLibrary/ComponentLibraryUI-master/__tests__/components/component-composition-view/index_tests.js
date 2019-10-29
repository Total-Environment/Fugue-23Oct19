import {shallow} from 'enzyme';
import React from 'react';
import {ComponentCompositionView, __RewireAPI__ as SFGCompositionViewRewired} from '../../../src/components/component-composition-view';
import chaiEnzyme from 'chai-enzyme';
import chai, {expect} from 'chai';

chai.use(chaiEnzyme());

describe('Component Composition View', () => {
  describe('render', () => {
    let props, wrapper;
    beforeEach(() => {
      props = {
        composition: {
          "componentCoefficients": [
            {
              "componentType": "Service",
              "unitOfMeasure": "m2",
              "name": "Short Description",
              "code": "FDP1027",
              "coefficient": 10,
              "wastagePercentages": [
                {
                  "name": "test3",
                  "value": 2
                },
                {
                  "name": "test4",
                  "value": 3
                }
              ],
              "totalWastagePercentage": 5,
              "totalQuantity": 10.5
            },
            {
              "componentType": "Material",
              "unitOfMeasure": "Nos",
              "name": "Aluminium Straight Edge",
              "code": "ALM000039",
              "coefficient": 10,
              "wastagePercentages": [
                {
                  "name": "test3",
                  "value": 110
                },
                {
                  "name": "test4",
                  "value": 20
                }
              ],
              "totalWastagePercentage": 130,
              "totalQuantity": 23
            }
          ]

        },
        cost: {
          "totalCost": {
            "currency": "INR",
            "value": 781746.636
          },
          "componentCostBreakup": [
            {
              "componentCode": "FDP1027",
              "cost": {
                "currency": "INR",
                "value": 409909.5
              }
            },
            {
              "componentCode": "ALM000039",
              "cost": {
                "currency": "INR",
                "value": 371837.136
              }
            }
          ]
        }
      };
      wrapper = shallow(<ComponentCompositionView {...props}/>);
    });

    it('should render all header names in sequence', () => {
      expect(wrapper.find('#composition-titles').find('div').at(1)).to.have.text('Resource Type');
      expect(wrapper.find('#composition-titles').find('div').at(2)).to.have.text('Component Code');
      expect(wrapper.find('#composition-titles').find('div').at(3)).to.have.text('Component Name');
      expect(wrapper.find('#composition-titles').find('div').at(4)).to.have.text('Unit of Measure');
      expect(wrapper.find('#composition-titles').find('div').at(5)).to.have.text('Coefficient');
      expect(wrapper.find('#composition-titles').find('div').at(6)).to.have.text('Handling/ Storage Wastage %');
      expect(wrapper.find('#composition-titles').find('div').at(7)).to.have.text('Execution Wastage %');
      expect(wrapper.find('#composition-titles').find('div').at(8)).to.have.text('Total Wastage %');
      expect(wrapper.find('#composition-titles').find('div').at(9)).to.have.text('Total Quantity');
      expect(wrapper.find('#composition-titles').find('div').at(10)).to.have.text('Rate');
    });

    it('should render column values', () => {
      expect(wrapper.find('#composition-values-0').find('div').at(1)).to.have.text('Service');
      expect(wrapper.find('#composition-values-0').find('div').at(2)).to.have.descendants('Link');
      expect(wrapper.find('#composition-values-0').find('div').at(2).find('Link').children()).to.have.text('FDP1027');
      expect(wrapper.find('#composition-values-0').find('div').at(2).find('Link')).to.have.prop('to', '/services/FDP1027');
      expect(wrapper.find('#composition-values-0').find('div').at(3)).to.have.text('Short Description');
      expect(wrapper.find('#composition-values-0').find('div').at(4)).to.have.text('m2');
      expect(wrapper.find('#composition-values-0').find('div').at(5)).to.have.text('10');
      expect(wrapper.find('#composition-values-0').find('div').at(6)).to.have.text('2');
      expect(wrapper.find('#composition-values-0').find('div').at(7)).to.have.text('3');
      expect(wrapper.find('#composition-values-0').find('div').at(8)).to.have.text('5');
      expect(wrapper.find('#composition-values-0').find('div').at(9)).to.have.text('10.5');
    });

    it('should render error message if error exists',() => {
      SFGCompositionViewRewired.__Rewire__('styles', {error: 'error', totalCostContainer: 'totalCostContainer'});
      props.error = 'error has occurred';
      wrapper = shallow(<ComponentCompositionView {...props}/>);
      expect(wrapper.find('span').last().prop('className')).to.equal('error');
      expect(wrapper).to.include.text('error has occurred');
    });

    it('should not render error message if error does not exists',() => {
      SFGCompositionViewRewired.__Rewire__('styles', {error: 'error', totalCostContainer: 'totalCostContainer'});
      wrapper = shallow(<ComponentCompositionView {...props}/>);
      expect(wrapper.find('span').last().prop('className')).to.not.equal('error');
    });
  });
});
