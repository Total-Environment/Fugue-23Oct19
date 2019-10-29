import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import sinon from 'sinon';
import chaiEnzyme from 'chai-enzyme';
import {Service} from '../../src/components/service';
import sinonChai from 'sinon-chai';
import React from 'react';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Service', () => {
  let serviceFetchRequestSpy, props, serviceShallow, onServiceRatesFetchRequestSpy;
  beforeEach(() => {
    serviceFetchRequestSpy = sinon.spy();
    onServiceRatesFetchRequestSpy = sinon.spy();
    props = {
      serviceCode: 'SR001',
      onServiceFetchRequest: serviceFetchRequestSpy,
      onServiceRatesFetchRequest: onServiceRatesFetchRequestSpy,
      error: ''
    };
    serviceShallow = shallow(<Service {...props}/>);
  });

  describe('ComponentDidMount', () => {
    it('should call onServiceFetchRequest when serviceDetails is null', () => {
      serviceShallow.instance().componentDidMount();
      expect(serviceFetchRequestSpy).to.be.calledWith('SR001');
    });

    it("should call onServiceRatesFetchRequest When rates is null", () => {
      serviceShallow.instance().componentDidMount();
      expect(onServiceRatesFetchRequestSpy).to.be.calledWith('SR001');
    });

    it('should call onServiceFetchRequest when serviceDetails is null but rates are present', () => {
      props.rates = 1;
      new Service(props).componentDidMount();
      expect(serviceFetchRequestSpy).to.be.calledWith('SR001');
    });
  });

  describe('render', () => {
    it('should call component when details are given', () => {
      props.details = {};
      props.classificationDefinition = 1;
      serviceShallow = shallow(<Service {...props}/>);
      serviceShallow.instance().render();
      expect(serviceShallow).to.have.descendants('Component');
      expect(serviceShallow.find('Component')).to.have.prop('details').deep.equal(props.details);
      expect(serviceShallow.find('Component')).to.have.prop('classificationDefinition').deep.equal(props.classificationDefinition);
    });

    it('should call Loading when details is not defined', () => {
      serviceShallow = shallow(<Service {...props}/>);
      serviceShallow.instance().render();
      expect(serviceShallow).to.have.descendants('Loading');
    });

    it('should call have div as child with proper service code when details is defined', () => {
      props.details = {
        headers: [
          {
            columns: [
              {
                name: 'Service Code',
                key: 'service_code',
                value: 'SR0001'
              },
              {
                name: 'Service Status',
                key: 'service_status',
                value: 'approved'
              }
            ],
            name: 'General',
            key: 'general'
          }
        ]
      };
      serviceShallow = shallow(<Service {...props}/>);
      serviceShallow.instance().render();
      expect(serviceShallow).to.have.descendants('h2');
      expect(serviceShallow.find('h2')).to.have.text('SR0001');
    });

    it('should return error when the error occurs', () => {
      props = {error: 'Service not found'};
      serviceShallow = shallow(<Service {...props}/>);
      serviceShallow.instance().render();
      expect(serviceShallow.find('h3').first()).to.have.text('Service not found');
    });

    it('should render a link to edit screen when props has details', () => {
      props.details = {general: {"service Code": {value: "SR001"}, "service Status": {value: "approved"}}};
      props.rates = [];
      serviceShallow = shallow(<Service {...props}/>);
      expect(serviceShallow).to.have.descendants('Link');
      expect(serviceShallow.find('Link')).to.have.prop('to', '/services/SR001/edit');
    });
  });
});
