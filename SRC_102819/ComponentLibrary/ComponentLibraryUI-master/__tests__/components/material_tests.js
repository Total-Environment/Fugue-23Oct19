import React from 'react';
import {shallow} from 'enzyme';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';

import {Material, __RewireAPI__ as MaterialRewired} from '../../src/components/material';

chai.use(sinonChai);
chai.use(chaiEnzyme());


describe('Material', () => {
  let onMaterialFetchRequestSpy, onMaterialRatesFetchRequestSpy, onRentalRatesFetchRequestSpy, props, wrapper;
  beforeEach(() => {
    onMaterialFetchRequestSpy = sinon.spy();
    onMaterialRatesFetchRequestSpy = sinon.spy();
    onRentalRatesFetchRequestSpy = sinon.spy();
    props = {
      materialCode: 'MT0001',
      onMaterialFetchRequest: onMaterialFetchRequestSpy,
      onMaterialRatesFetchRequest: onMaterialRatesFetchRequestSpy,
      onRentalRatesFetchRequest: onRentalRatesFetchRequestSpy,
      error: ''
    };
    wrapper = shallow(<Material {...props}/>);
  });

  describe('componentWillReceiveProps', () => {
    it('should call onRentalRateFetch when canbeUsedAnAsset is true', () => {
      wrapper.setProps({
        details: {
          headers: [
            {
              columns: [
                {
                  name: 'Material Name',
                  key: 'material_name',
                  value: 'sample'
                },
                {
                  name: 'Material Code',
                  key: 'material_code',
                  value: 'MT0001'
                },
                {
                  name: 'Material Status',
                  key: 'material_status',
                  value: 'Approved'
                },
                {
                  name: 'can be Used as an Asset',
                  key: 'can_be_used_as_an_asset',
                  value: true
                }
              ],
              name: 'General',
              key: 'general',
            },
          ]
        }
      });
      expect(onRentalRatesFetchRequestSpy).to.be.calledWith("MT0001");
    });

    it('Should not call onRentalRatesFetchRequest when canbeUsedAnAsset as false', () => {
      wrapper.setProps({
        details: {
          headers: [
            {
              columns: [
                {
                  name: 'Material Name',
                  key: 'material_name',
                  value: 'sample'
                },
                {
                  name: 'Material Code',
                  key: 'material_code',
                  value: 'MT0001'
                },
                {
                  name: 'Material Status',
                  key: 'material_status',
                  value: 'Approved'
                },
                {
                  name: 'can be Used as an Asset',
                  key: 'can_be_used_as_an_asset',
                  value: false
                }
              ],
              name: 'General',
              key: 'general',
            },
          ]
        }
      });
      expect(onRentalRatesFetchRequestSpy).to.not.be.called;
    });

  });
  describe('componentDidMount', () => {
    it("should call onMaterialFetchRequest When It doesn't have material details", () => {
      wrapper.instance().componentDidMount();
      expect(onMaterialFetchRequestSpy).to.be.calledWith('MT0001');
    });
    it("should call onMaterialRatesFetchRequest When It doen't have material rates", () => {
      wrapper.instance().componentDidMount();
      expect(onMaterialRatesFetchRequestSpy).to.be.calledWith('MT0001');
    });
    it('should call onRentalRatesFetchRequest when canBeUsedAsAnAsset is true', () => {
      props.details = {
        headers: [
          {
            columns: [
              {
                name: 'Material Name',
                key: 'material_name',
                value: 'sample'
              },
              {
                name: 'Material Code',
                key: 'material_code',
                value: 'MT0001'
              },
              {
                name: 'Material Status',
                key: 'material_status',
                value: 'Approved'
              },
              {
                name: 'can be Used as an Asset',
                key: 'can_be_used_as_an_asset',
                value: true
              }
            ],
            name: 'General',
            key: 'general',
          },
        ]
      };
      wrapper = shallow(<Material {...props}/>);
      wrapper.instance().componentDidMount();
      expect(onRentalRatesFetchRequestSpy).to.be.calledWith('MT0001');
    });
  });

  describe('render', () => {
    it('should render Component when details are present', () => {
      props.details = {
        headers: [
          {
            columns: [
              {
                name: 'Material Name',
                key: 'material_name',
                value: 'sample'
              },
              {
                name: 'Material Code',
                key: 'material_code',
                value: 'MT0001'
              },
              {
                name: 'Material Status',
                key: 'material_status',
                value: 'Approved'
              },
              {
                name: 'can be Used as an Asset',
                key: 'can_be_used_as_an_asset',
                value: true
              }
            ],
            name: 'General',
            key: 'general',
          },
        ]
      };
      props.rates = {};
      props.rentalRates = 'Rental Rates';
      props.rentalRatesError = 'Rental Rates Error';
      wrapper = shallow(<Material {...props}/>);
      expect(wrapper).to.have.descendants('Component');
      expect(wrapper.find('Component')).to.have.prop('details').deep.equal(props.details);
      expect(wrapper.find('Component')).to.have.prop('rates').deep.equal(props.rates);
      expect(wrapper.find('Component')).to.have.prop('rentalRates').deep.equal(props.rentalRates);
      expect(wrapper.find('Component')).to.have.prop('rentalRatesError').deep.equal(props.rentalRatesError);
    });

    it('should have the material code and material name as title', () => {
      props.details = {
        headers: [
          {
            columns: [
              {
                name: 'Material Name',
                key: 'material_name',
                value: 'Name'
              },
              {
                name: 'Material Code',
                key: 'material_code',
                value: 'CLY001'
              },
              {
                name: 'Material Status',
                key: 'material_status',
                value: 'Approved'
              },
              {
                name: 'can be Used as an Asset',
                key: 'can_be_used_as_an_asset',
                value: true
              }
            ],
            name: 'General',
            key: 'general',
          },
        ]
      };
      props.rates = [];
      wrapper = shallow(<Material {...props}/>);
      expect(wrapper).to.have.descendants('h2');
      expect(wrapper.find('h2')).to.have.text("Name | CLY001");
    });

    it('should have the material status', () => {
      props.details = {
        headers: [
          {
            columns: [
              {
                name: 'Material Name',
                key: 'material_name',
                value: 'Name'
              },
              {
                name: 'Material Code',
                key: 'material_code',
                value: 'CLY001'
              },
              {
                name: 'Material Status',
                key: 'material_status',
                value: 'Approved'
              },
              {
                name: 'can be Used as an Asset',
                key: 'can_be_used_as_an_asset',
                value: true
              }
            ],
            name: 'General',
            key: 'general',
          },
        ]
      };
      props.rates = [];
      MaterialRewired.__Rewire__('styles', {approved: 'approved'});
      wrapper = shallow(<Material {...props}/>);
      expect(wrapper.find('ComponentStatus')).to.have.length(1);
      expect(wrapper.find('ComponentStatus')).to.have.prop('value').to.equal('Approved');
    });
    it('should render affix', () => {
      props.details = {
        headers: [
          {
            columns: [
              {
                name: 'Material Name',
                key: 'material_name',
                value: 'Name'
              },
              {
                name: 'Material Code',
                key: 'material_code',
                value: 'CLY001'
              },
              {
                name: 'Material Status',
                key: 'material_status',
                value: 'Approved'
              },
              {
                name: 'can be Used as an Asset',
                key: 'can_be_used_as_an_asset',
                value: true
              }
            ],
            name: 'General',
            key: 'general',
          },
        ]
      };
      props.rates = [];
      wrapper = shallow(<Material {...props}/>);
      expect(wrapper).to.have.descendants('AutoAffix');
    });

    it('should render loading when details are not present', () => {
      expect(wrapper).to.have.descendants('Loading');
    });

    it('should render error message when props has error message', () => {
      props = {detailserror: "Material not found"};
      wrapper = shallow(<Material {...props} />);
      expect(wrapper.find('h3').first()).to.have.text('Material not found');
    });

    it('should render a link to edit screen when props has details', () => {
      props.details = {
        headers: [
          {
            columns: [
              {
                name: 'Material Name',
                key: 'material_name',
                value: 'Name'
              },
              {
                name: 'Material Code',
                key: 'material_code',
                value: 'CLY001'
              },
              {
                name: 'Material Status',
                key: 'material_status',
                value: 'Approved'
              },
              {
                name: 'can be Used as an Asset',
                key: 'can_be_used_as_an_asset',
                value: true
              }
            ],
            name: 'General',
            key: 'general',
          },
        ]
      };
      props.rates = [];
      wrapper = shallow(<Material {...props}/>);
      expect(wrapper).to.have.descendants('Link');
      expect(wrapper.find('Link')).to.have.prop('to', '/materials/MT0001/edit');
    });
  });
});
