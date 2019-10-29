import {Checklist} from '../../src/components/checklists/checklist';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import sinon from 'sinon';
import {shallow} from 'enzyme';
import React from 'react';

chai.use(chaiEnzyme());

describe('Checklist', () => {
  it('should render a QuantityEvaluationMethod if checklistDetails exists and template is QuantityEvaluationMethod', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'SRID0001',
      checklistDetails: {
        checkListId: 'SRID0001',
        id: '23',
        name: 'abc',
        template: 'Inspection Checklist',
        content: {entries: [{cells: [{value: 'S.No', key: '0 0'}]}]}
      },
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('QuantityEvaluationMethod');
    expect(wrapper).to.not.have.descendants('SpecificationsSheet');
    const quantityEvaluationMethod = wrapper.find('QuantityEvaluationMethod');
    expect(quantityEvaluationMethod).to.have.prop('details').deep.equal(props.checklistDetails);
  });

  it('should render a SpecificationsSheet if checklistDetails exists and template is SpecificationsSheet', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'SRID0001',
      checklistDetails: {
        checkListId: 'SRID0001',
        id: '23',
        name: 'abc',
        template: 'Specification Sheet',
        content: {entries: [{cells: [{value: 'S.No', key: '0 0'}]}]}
      },
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('SpecificationsSheet');
    expect(wrapper).to.not.have.descendants('QuantityEvaluationMethod');
    const SpecificationsSheet = wrapper.find('SpecificationsSheet');
    expect(SpecificationsSheet).to.have.prop('details').deep.equal(props.checklistDetails);
  });

  it('should render a GeneralPoTermsAndConditions if checklistDetails exists and template is GeneralPoTerms', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'SRID0001',
      checklistDetails: {
        checkListId: 'SRID0001',
        id: '23',
        template: 'General PO Terms',
        content: {entries: [{cells: [{value: 'S.No', key: '0 0'}]}]}
      },
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('GeneralTermsAndConditions');
    expect(wrapper).to.not.have.descendants('QuantityEvaluationMethod');
    expect(wrapper).to.not.have.descendants('SpecificationsSheet');
    const generalPoTermsAndConditions = wrapper.find('GeneralTermsAndConditions');
    expect(generalPoTermsAndConditions).to.have.prop('details').deep.equal(props.checklistDetails);
  });

  it('should render a GeneralSoTermsAndConditions if checklistDetails exists and template is GeneralSoTerms', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'STD001',
      checklistDetails: {
        checkListId: 'STD001',
        id: '69',
        template: 'General SO Terms',
        content: {entries: [{cells: [{value: 'S.No', key: '0 0'}]}]}
      },
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('GeneralTermsAndConditions');
    expect(wrapper).to.not.have.descendants('QuantityEvaluationMethod');
    expect(wrapper).to.not.have.descendants('SpecificationsSheet');
    const generalSoTermsAndConditions = wrapper.find('GeneralTermsAndConditions');
    expect(generalSoTermsAndConditions).to.have.prop('details').deep.equal(props.checklistDetails);
  });

  it('should render a SpecialPoTermsAndConditions if checklistDetails exists and template is SpecialPoTerms', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'SRID0001',
      checklistDetails: {
        checkListId: 'SRID0001',
        id: '23',
        template: 'Special PO Terms',
        content: {entries: [{cells: [{value: 'S.No', key: '0 0'}]}]}
      },
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('SpecialTermsAndConditions');
    expect(wrapper).to.not.have.descendants('QuantityEvaluationMethod');
    expect(wrapper).to.not.have.descendants('SpecificationsSheet');
    expect(wrapper).to.not.have.descendants('GeneralTermsAndConditions');
    const specialPoTermsAndConditions = wrapper.find('SpecialTermsAndConditions');
    expect(specialPoTermsAndConditions).to.have.prop('details').deep.equal(props.checklistDetails);
  });

  it('should render a SpecialSoTermsAndConditions if checklistDetails exists and template is SpecialSoTerms', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'SP0001',
      checklistDetails: {
        checkListId: 'SP0001',
        id: '23',
        template: 'Special SO Terms',
        content: {entries: [{cells: [{value: 'S.No', key: '0 0'}]}]}
      },
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('SpecialTermsAndConditions');
    expect(wrapper).to.not.have.descendants('QuantityEvaluationMethod');
    expect(wrapper).to.not.have.descendants('SpecificationsSheet');
    expect(wrapper).to.not.have.descendants('GeneralTermsAndConditions');
    const specialSoTermsAndConditions = wrapper.find('SpecialTermsAndConditions');
    expect(specialSoTermsAndConditions).to.have.prop('details').deep.equal(props.checklistDetails);
  });

  it('should render a QualityChecklist if checklistDetails exists and template is QualityChecklist', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'SP0001',
      checklistDetails: {
        checkListId: 'SP0001',
        id: '23',
        template: 'QualityChecklist',
        content: {entries: [{cells: [{value: 'S.No', key: '0 0'}]}]}
      },
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('QualityChecklist');
    expect(wrapper).to.not.have.descendants('SpecialTermsAndConditions');
    expect(wrapper).to.not.have.descendants('QuantityEvaluationMethod');
    expect(wrapper).to.not.have.descendants('SpecificationsSheet');
    expect(wrapper).to.not.have.descendants('GeneralTermsAndConditions');
    const qualityChecklist = wrapper.find('QualityChecklist');
    expect(qualityChecklist).to.have.prop('details').deep.equal(props.checklistDetails);
  });

  it('should render a error message if checklist does not exist', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'SRID0001',
      checklistDetails: null,
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('h3');
    const errorMessage = wrapper.find('h3');
    expect(errorMessage.contains('Loading...')).to.equal(true);
  });

  it('should render a error message if checklist exists and invalid template is given', () => {
    let onChecklistFetchRequestSpy = sinon.spy();
    let props = {
      checklistId: 'SRID0001',
      checklistDetails: {},
      onChecklistFetchRequest: onChecklistFetchRequestSpy,
    };
    let wrapper = shallow(<Checklist {...props} />);
    expect(wrapper).to.have.descendants('h3');
    const errorMessage = wrapper.find('h3');
    expect(errorMessage.contains('Unable to render checklist')).to.equal(true);
  });

  describe('componentDidMount', () => {
    it('should not dispatch action if checklistDetails are there', () => {
      let onChecklistFetchRequestSpy = sinon.spy();

      let props = {
        checklistId: 'SRID0001',
        checklistDetails: {},
        onChecklistFetchRequest: onChecklistFetchRequestSpy,
      };
      let wrapper = shallow(<Checklist {...props} />);
      wrapper.instance().componentDidMount();
      expect(onChecklistFetchRequestSpy.called).to.be.false;
    });

    it('should dispatch action if checklistDetails is null', () => {
      let onChecklistFetchRequestSpy = sinon.spy();

      let props = {
        checklistId: 'SRID0001',
        checklistDetails: null,
        onChecklistFetchRequest: onChecklistFetchRequestSpy,
      };
      let wrapper = shallow(<Checklist {...props} />);
      wrapper.instance().componentDidMount();
      expect(onChecklistFetchRequestSpy.called).to.be.true;
    });
  });
});
